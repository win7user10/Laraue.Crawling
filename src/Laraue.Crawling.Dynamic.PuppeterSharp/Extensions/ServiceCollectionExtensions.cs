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
    /// Add <see cref="IBrowserFactory"/> implementation to the container.
    /// </summary>
    /// <param name="serviceCollection"></param>
    /// <param name="launchOptions"></param>
    /// <returns></returns>
    public static IServiceCollection AddPuppeterFactory(
        this IServiceCollection serviceCollection,
        LaunchOptions launchOptions)
    {
        return serviceCollection.AddSingleton<IBrowserFactory>(new BrowserFactory(launchOptions));
    }
}