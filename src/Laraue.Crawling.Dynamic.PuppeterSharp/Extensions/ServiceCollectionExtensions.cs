using Laraue.Crawling.Abstractions;
using Laraue.Crawling.Dynamic.PuppeterSharp.Abstractions;
using Laraue.Crawling.Dynamic.PuppeterSharp.Utils;
using Microsoft.Extensions.DependencyInjection;
using PuppeteerSharp;

namespace Laraue.Crawling.Dynamic.PuppeterSharp.Extensions;

/// <summary>
/// Extensions for Microsoft container.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add puppeteer crawling services to the container.
    /// </summary>
    /// <param name="serviceCollection"></param>
    /// <param name="launchOptions"></param>
    /// <returns></returns>
    public static IServiceCollection AddCrawlingServices(
        this IServiceCollection serviceCollection,
        LaunchOptions launchOptions)
    {
        return serviceCollection
            .AddSingleton<IBrowserFactory>(new BrowserFactory(launchOptions))
            .AddSingleton<IHtmlSchemaParser<IElementHandle>, PuppeterSharpParser>()
            .AddSingleton<IPageParser, PageParser>();
    }
}