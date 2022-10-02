namespace Laraue.Crawling.Dynamic;

/// <summary>
/// Class which provides links to be parsed.
/// </summary>
public interface IPagesProvider
{
    /// <summary>
    /// Returns stream that contain page urls should be parsed.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    IAsyncEnumerable<Uri> GetPages(CancellationToken cancellationToken);
}