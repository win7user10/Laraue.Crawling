using PuppeteerSharp;

namespace Laraue.Crawling.Dynamic.PuppeterSharp.Utils;

/// <summary>
/// Abstraction to get browser instances.
/// </summary>
public interface IBrowserFactory
{
    /// <summary>
    /// Get browser instance from the factory.
    /// </summary>
    /// <returns></returns>
    ValueTask<IBrowser> GetInstanceAsync();
}