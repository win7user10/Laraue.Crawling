using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using PuppeteerSharp;
using Xunit;

namespace Laraue.Crawling.Dynamic.Tests;

public class CianCrawlerTests
{
    private readonly TestDbContext _dbContext;
    
    public CianCrawlerTests()
    {
        _dbContext = new ContextFactory().CreateDbContext(Array.Empty<string>());
        
        _dbContext.Database.Migrate();
    }
    
    [Fact]
    public async Task TestCianParserTest()
    {
        using var browserFetcher = new BrowserFetcher();
        await browserFetcher.DownloadAsync();
        await using var browser = await Puppeteer.LaunchAsync(
            new LaunchOptions
            {
                Headless = false
            });
        
        var crawler = new CianCrawler(browser, new NullLogger<CianCrawler>());
        await foreach (var pages in await crawler.RunAsync())
        {
            var exists = _dbContext.CianPages
                .Where(x => pages.Pages.Select(y => y.Id).Contains(x.Id))
                .Select(x => x.Id)
                .ToHashSet();
            
            foreach (var page in pages.Pages)
            {
                if (!exists.Contains(page.Id))
                {
                    _dbContext.CianPages.Add(page);
                }
            }

            await _dbContext.SaveChangesAsync();
        }
    }
}