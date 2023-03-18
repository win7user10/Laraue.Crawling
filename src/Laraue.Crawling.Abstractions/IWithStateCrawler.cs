namespace Laraue.Crawling.Abstractions;

/// <summary>
/// The <see cref="ICrawler{TModel}"/> that contains a state that can be stored somehow.
/// </summary>
/// <typeparam name="TModel"></typeparam>
/// <typeparam name="TState"></typeparam>
public interface IWithStateCrawler<out TModel, out TState> : ICrawler<TModel>
    where TModel : class
{
    /// <summary>
    /// Current state of the crawler.
    /// </summary>
    TState? CrawlingState { get; }

    /// <summary>
    /// Load the initial state of the crawler.
    /// </summary>
    /// <returns></returns>
    Task InitializeAsync();
    
    /// <summary>
    /// Save current crawler state.
    /// </summary>
    /// <returns></returns>
    Task SaveStateAsync();

    /// <summary>
    /// Reset state to the default.
    /// </summary>
    /// <returns></returns>
    Task ResetStateAsync();
}

// TODO - next abstraction is paginable crawler