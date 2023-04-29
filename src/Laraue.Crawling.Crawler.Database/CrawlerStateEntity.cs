namespace Laraue.Crawling.Crawler.Database;

/// <summary>
/// Table for crawlers states.
/// </summary>
public sealed class CrawlerStateEntity
{
    /// <summary>
    /// Unique crawler key.
    /// </summary>
    public string Key { get; init; } = default!;

    /// <summary>
    /// Serialized crawler state.
    /// </summary>
    public string State { get; init; } = default!;
}