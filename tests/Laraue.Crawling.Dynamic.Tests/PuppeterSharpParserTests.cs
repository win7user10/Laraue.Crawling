using Laraue.Crawling.Common.Extensions;
using Laraue.Crawling.Common.Impl;
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
    public async Task AttributeValue_ShouldBeBindCorrectlyAsync()
    {
        var model = await TestAsync<Model>(
            htmlTemplate: "<meta itemprop=price content=11500000></meta>",
            builder => builder
                .HasProperty(x => x.StringValue,  htmlSelector: "meta[itemprop=price]", attributeName: "content"));
        
        Assert.Equal("11500000", model.StringValue);
    }

    [Fact]
    public async Task AttributeValue_ShouldBeBindCorrectly_ViaImplicitCastAsync()
    {
        var model = await TestAsync<Model>(
            htmlTemplate: "<meta itemprop=price content=11500000></meta>",
            builder => builder
                .HasProperty(x => x.LongValue,  htmlSelector: "meta[itemprop=price]", attributeName: "content"));
        
        Assert.Equal(11500000, model.LongValue);
    }
    
    [Fact]
    public async Task AttributeValue_ShouldBeBindCorrectly_ViaMapFunctionCastAsync()
    {
        var model = await TestAsync<Model>(
            htmlTemplate: "<meta itemprop=price content=11500000RUB></meta>",
            builder => builder
                .HasProperty(
                    x => x.LongValue,
                    htmlSelector: "meta[itemprop=price]",
                    attributeName: "content",
                    getValue: s => long.Parse(s.GetOnlyDigits())));
        
        Assert.Equal(11500000, model.LongValue);
    }
    
    [Fact]
    public async Task AttributeValue_ShouldBeTakenCorrectlyWhenSelectorIsNotPassedAsync()
    {
        var model = await TestAsync<Model>(
            htmlTemplate: "<meta itemprop=price content=11500000></meta>",
            builder => builder
                .HasObjectProperty(
                    x => x.ChildValue,
                    htmlSelector: "meta[itemprop=price]",
                    childBuilder =>
                    {
                        childBuilder.HasProperty(
                            x => x.StringValue,
                            htmlSelector: null,
                            attributeName: "content");
                    }));
        
        Assert.Equal("11500000", model.ChildValue.StringValue);
    }

    private async Task<TModel> TestAsync<TModel>(
        string htmlTemplate,
        Action<PuppeterSharpSchemaBuilder<TModel>> buildSchema)
        where TModel : class, ICrawlingModel
    {
        var fullTemplate = $"<html><body>{htmlTemplate}</body></html>";

        var schemaBuilder = new PuppeterSharpSchemaBuilder<TModel>();

        buildSchema(schemaBuilder);
        
        await LoadHtmlAsync(fullTemplate);
        
        return await _parser.RunAsync(schemaBuilder.Build(), await _page.QuerySelectorAsync("body"));
    }

    private Task LoadHtmlAsync(string html)
    {
        return _page.SetContentAsync(html);
    }
        
    private sealed class Model : ICrawlingModel
    {
        public string StringValue { get; init; }
        public long LongValue { get; init; }

        public Model ChildValue { get; init; }
    }

    public async Task DisposeAsync()
    {
        await _browser.DisposeAsync();
    }
}