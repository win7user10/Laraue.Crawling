namespace Laraue.Crawling.Abstractions;

/// <summary>
/// The wrapper for simple crawling cases when the simple value should be parsed.
/// </summary>
/// <typeparam name="TCrawlingElement"></typeparam>
public sealed class GenericCrawlingModel<TCrawlingElement> : ICrawlingModel
{
    /// <summary>
    /// Simple type crawling result.
    /// </summary>
    public TCrawlingElement? Value { get; set; }
}