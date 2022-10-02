namespace Laraue.Crawling.Abstractions;

/// <summary>
/// Represents the class can be used for crawling of the information.
/// </summary>
/// <typeparam name="TModel"></typeparam>
public interface ICrawler<TModel>
    where TModel : class
{
    /// <summary>
    /// Run the crawling process.
    /// </summary>
    /// <returns></returns>
    Task<IAsyncEnumerable<TModel>> RunAsync(CancellationToken cancellationToken = default);
}