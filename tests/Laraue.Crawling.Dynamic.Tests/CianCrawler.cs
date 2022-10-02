using Laraue.Crawling.Common.Extensions;
using Laraue.Crawling.Dynamic.C;
using Laraue.Crawling.Dynamic.PuppeterSharp;
using Microsoft.Extensions.Logging;
using PuppeteerSharp;

namespace Laraue.Crawling.Dynamic.Tests;

public class CianCrawler : BaseFileCrawler<CianPages, string, CrawlerState>
{
    private readonly Browser _browser;
    private readonly ILogger<CianCrawler> _logger;
    protected override string StateFilePath => "cian_crawler_state.json";

    public CianCrawler(Browser browser, ILogger<CianCrawler> logger)
    {
        _browser = browser;
        _logger = logger;
    }
    
    protected override async IAsyncEnumerable<string> GetLinks()
    {
        while (true)
        {
            CrawlingState.LastPage++;
            
            yield return
                $"https://spb.cian.ru/cat.php?deal_type=sale&engine_version=2&offer_type=flat&p={CrawlingState.LastPage}&region=2";

            await SaveStateAsync();
        }
        // ReSharper disable once IteratorNeverReturns
    }

    protected override async IAsyncEnumerable<CianPages> ParsePages(IAsyncEnumerable<string> links)
    {
        var schema = new DynamicHtmlSchemaBuilder<CianPages>()
            .ParseArrayProperty(x => x.Pages, "article", pageBuilder =>
            {
                pageBuilder.BindExactly(async (_, element, modelBinder) =>
                {
                    var titleElement = await element.QuerySelectorAsync("span[data-mark=OfferTitle]");
                    var subTitleElement = await element.QuerySelectorAsync("span[data-mark=OfferSubtitle]");
                    
                    var title = titleElement is not null ? await titleElement.GetInnerTextAsync() : string.Empty;
                    var subTitle = subTitleElement is not null ? await subTitleElement.GetInnerTextAsync() : string.Empty;

                    var titleRate = GetMatchTitleRate(title);
                    var subTitleRate = GetMatchTitleRate(subTitle);

                    var stringToParse = titleRate < subTitleRate ? subTitle : title;
                    var textParts = stringToParse.Split(',');

                    if (textParts.Length != 4)
                    {
                        return;
                    }

                    var roomsCount = textParts[0].GetIntOrDefault();
                    var square = $"{textParts[1]}.{textParts[2][..2]}".GetDecimalOrDefault();

                    var floorParts = textParts[3].Split("/");
                    var floor = floorParts[0].GetIntOrDefault();
                    var totalFloors = floorParts[1].GetIntOrDefault();
                    
                    modelBinder.BindProperty(x => x.RoomsCount, roomsCount);
                    modelBinder.BindProperty(x => x.Square, square);
                    modelBinder.BindProperty(x => x.FloorNumber, floor);
                    modelBinder.BindProperty(x => x.TotalFloorsNumber, totalFloors);
                });

                pageBuilder.BindExactly(async (_, element, modelBinder) =>
                {
                    var subElement = await element.QuerySelectorAsync("div[data-name=SpecialGeo] > div");
                    if (subElement is null)
                    {
                        return;
                    }
                    
                    // 7 минут пешком or 5 минут на транспорте
                    var title = await subElement.GetInnerTextAsync();
                    var titleParts = title.Split(' ');

                    var minutesToMetro = titleParts[0].GetIntOrDefault();
                    var distanceType = titleParts.Last() == "пешком" ? DistanceType.Walk : DistanceType.Car;
                    
                    modelBinder.BindProperty(x => x.MinutesToStop, minutesToMetro);
                    modelBinder.BindProperty(x => x.TransportDistanceType, distanceType);
                });
                
                pageBuilder.ParseProperty(x => x.PublicTransportStop, "div[data-name=SpecialGeo] a");

                pageBuilder.ParseProperty(x => x.Id, "div[data-name=LinkArea] a", handle
                    => handle.GetAttributeValueAsync("href").AwaitAndModify(x => x.GetIntOrDefault()));
                pageBuilder.ParseProperty(x => x.TotalPrice, "span[data-mark=MainPrice]");
                pageBuilder.ParseProperty(x => x.OneMeterPrice, "p[data-mark=PriceInfo]");
            })
            .Build();
        
        var parser = new PuppeterSharpParser();
        await using var page = await _browser.NewPageAsync();
        await page.SetViewportAsync(new ViewPortOptions()
        {
            Height = 1920,
            Width = 1080,
            IsMobile = false,
        });
        
        await foreach (var link in links)
        {
            var response = await page.GoToAsync(link);
            
            if (response.Url != link)
            {
                yield break;
            }
            
            yield return await parser.VisitAsync(page, schema);
        }
    }

    private int GetMatchTitleRate(string str)
    {
        return rateStrings.Count(rateString => str.Contains(rateString, StringComparison.InvariantCultureIgnoreCase));
    }

    private static readonly HashSet<string> rateStrings = new()
    {
        "комн. кв",
        "студия",
        "м²",
        "этаж",
    };
}

public record CrawlerState
{
    public int LastPage { get; set; }
}

public record CianPages
{
    public CianPage[] Pages { get; init; }
}

public record CianPage
{
    public int Id { get; init; }

    public int TotalPrice { get; init; }

    public int OneMeterPrice { get; init; }

    public int RoomsCount { get; init; }

    public decimal Square { get; init; }

    public int FloorNumber { get; init; }
    
    public int TotalFloorsNumber { get; init; }

    public string? PublicTransportStop { get; init; }
    
    public int? MinutesToStop { get; init; }

    public DistanceType? TransportDistanceType { get; init; }
}

public enum DistanceType
{
    Walk,
    Car,
}