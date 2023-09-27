using Laraue.Crawling.Abstractions;
using Laraue.Crawling.Dynamic.PuppeterSharp.Abstractions;
using Laraue.Crawling.Dynamic.PuppeterSharp.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
            .AddSingleton<IBrowserFactory>(sp => new BrowserFactory(launchOptions, sp.GetRequiredService<ILoggerFactory>()))
            .AddSingleton<IDocumentSchemaParser<IElementHandle, HtmlSelector>, PuppeterSharpParser>()
            .AddSingleton<IPageParser, PageParser>();
    }
}