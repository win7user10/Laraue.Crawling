using System.Text.Json;
using Laraue.Crawling.Dynamic.PuppeterSharp;
using PuppeteerSharp;
using Xunit;
using Xunit.Abstractions;

namespace Laraue.Crawling.Dynamic.Tests;

public class PuppeteerSharpParserTests
{
    private readonly ITestOutputHelper _outputHelper;

    public PuppeteerSharpParserTests(ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;
    }
    
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

        var fileName = "links.json";
        var categoryLinks = new List<Link>();
        if (!File.Exists(fileName))
        {
            await page.GoToAsync("https://www.wildberries.ru/catalog/29390438/detail.aspx?targetUrl=GP");

            await page.ClickAsync(".nav-element__burger");
            await Task.Delay(1000);

            var firstLevelLinks = await page.QuerySelectorAllAsync(".menu-burger__main-list-item");
            foreach (var firstLevelLink in firstLevelLinks)
            {
                try
                {
                    await firstLevelLink.HoverAsync();
                    await Task.Delay(10);
                }
                catch (Exception e)
                {
                    _outputHelper.WriteLine(e.Message);
                    continue;
                }

                await using var secondLevelMenu = await page.QuerySelectorAsync(".menu-burger__drop-list-item--active .menu-burger__set");
                if (secondLevelMenu is null)
                {
                    continue;
                }
                
                var secondLevelLinks = await secondLevelMenu.QuerySelectorAllAsync("a");
                foreach (var secondLevelLink in secondLevelLinks)
                {
                    var link = await secondLevelLink.GetAttributeValueAsync("href");
                    var name = await secondLevelLink.GetTrimmedInnerTextAsync();

                    categoryLinks.Add(new Link(name, link));
                    
                    await secondLevelLink.DisposeAsync();
                }

                var secondLevelMenuItemsLength = await secondLevelMenu.GetElementsCountAsync("span");
                await using var bodyElement = await page.QuerySelectorAsync("body");

                for (var i = 0; i < secondLevelMenuItemsLength; i++)
                {
                    await using var secondLevelMenuItem = await bodyElement.QuerySelectorByIndexAsync(
                        ".menu-burger__drop-list-item--active .menu-burger__set span",
                        i);
                    
                    await secondLevelMenuItem.ClickAsync();
                    var thirdLevelLinks = await page.QuerySelectorAllAsync(
                        ".menu-burger__drop-list-item--active .menu-burger__second.menu-burger__second--active a");
                    foreach (var thirdLevelLink in thirdLevelLinks)
                    {
                        var link = await thirdLevelLink.GetAttributeValueAsync("href");
                        var text = await thirdLevelLink.GetTrimmedInnerTextAsync();
                        categoryLinks.Add(new Link(text, link));

                        await thirdLevelLink.DisposeAsync();
                    }
                }
            }
            
            JsonSerializer.Serialize(categoryLinks);
        }
        else
        {
            categoryLinks = JsonSerializer.Deserialize<List<Link>>(File.OpenRead(fileName));
        }

        foreach (var categoryLink in categoryLinks)
        {
            
        }
    }
}

public record Link(string Name, string Url);

public record WildberriesProductPage
{
    public string Category { get; init; }
    public string Title { get; init; }
    public int PurchasesCount { get; init; }
    public int Articul { get; init; }
    public int Price { get; init; }
    public decimal Rating { get; init; }
    public StatGroup[] StatsGroups { get; init; }
}

public record StatGroup(string Name, Stat[] Stats);
public record Stat(string Name, string Value);