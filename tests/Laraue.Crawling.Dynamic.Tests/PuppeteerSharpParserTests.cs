using Laraue.Crawling.Dynamic.PuppeterSharp;
using PuppeteerSharp;
using Xunit;

namespace Laraue.Crawling.Dynamic.Tests;

public class PuppeteerSharpParserTests
{
    [Fact]
    public async Task SchemaAsync()
    {
        using var browserFetcher = new BrowserFetcher();
        await browserFetcher.DownloadAsync();
        await using var browser = await Puppeteer.LaunchAsync(
            new LaunchOptions
            {
                Headless = false
            });
        await using var page = await browser.NewPageAsync();
        await page.SetViewportAsync(new ViewPortOptions()
        {
            Height = 1920,
            Width = 1080,
            IsMobile = false,
        });
        await page.GoToAsync("https://www.wildberries.ru/catalog/29390438/detail.aspx?targetUrl=GP");
        
        var schema = new DynamicHtmlSchemaBuilder<WildberriesProductPage>()
            .ExecuteAsync(page => page.ClickAsync(".j-wba-card-item.j-wba-card-item-show"))
            .ParseArrayProperty(x => x.StatsGroups, ".product-params__table", statGroupsBuilder =>
            {
                statGroupsBuilder.ParseProperty(x => x.Name, ".product-params__caption", element => element.GetTrimmedInnerHtmlAsync())
                    .ParseArrayProperty(x => x.Stats, "tr", statsBuilder =>
                    {
                        statsBuilder.ParseProperty(x => x.Name, "th", element => element.GetTrimmedInnerHtmlAsync())
                            .ParseProperty(x => x.Value, "td", element => element.GetTrimmedInnerHtmlAsync());
                    });
            })
            .Build();

        var parser = new PuppeterSharpParser();
        var result = await parser.VisitAsync(page, schema);
    }
}

record WildberriesProductPage(StatGroup[] StatsGroups);
record StatGroup(string Name, Stat[] Stats);
record Stat(string Name, string Value);