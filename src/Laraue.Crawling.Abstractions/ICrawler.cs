namespace Laraue.Crawling.Abstractions;

public interface ICrawler<TModel>
    where TModel : class
{
    /// <summary>
    /// Run parsing.
    /// </summary>
    /// <returns></returns>
    Task<IAsyncEnumerable<TModel>> RunAsync();
}