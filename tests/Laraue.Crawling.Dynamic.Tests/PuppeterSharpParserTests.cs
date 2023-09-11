using Laraue.Crawling.Dynamic.PuppeterSharp;
using Microsoft.Extensions.Logging;
using PuppeteerSharp;
using Xunit;

namespace Laraue.Crawling.Dynamic.Tests;

public sealed class PuppeterSharpParserTests : IAsyncLifetime
{
    private readonly PuppeterSharpParser _parser;
    private IBrowser _browser;
    private IPage _page;

    public PuppeterSharpParserTests()
    {
        _parser = new PuppeterSharpParser(new LoggerFactory());
    }
    
    public async Task InitializeAsync()
    {
        await new BrowserFetcher().DownloadAsync();
        _browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true });
        _page = await _browser.NewPageAsync();
    }
    
    [Fact]
    public async Task AttributeShouldBeParsedCorrectlyAsync()
    {
        await LoadHtmlAsync("<html><body><meta itemprop=price content=11500000></meta></body></html>");

        var schema = new PuppeterSharpSchemaBuilder<Model>();
        schema.HasProperty(x => x.Price,  htmlSelector: "meta[itemprop=price]", attributeName: "content");
        
        var model = await _parser.RunAsync(schema.Build(), await _page.QuerySelectorAsync("body"));
        
        Assert.Equal("11500000", model.Price);
    }

    private Task LoadHtmlAsync(string html)
    {
        return _page.SetContentAsync(html);
    }
        
    private sealed class Model
    {
        public string Price { get; init; }
    }

    public async Task DisposeAsync()
    {
        await _browser.DisposeAsync();
    }
}