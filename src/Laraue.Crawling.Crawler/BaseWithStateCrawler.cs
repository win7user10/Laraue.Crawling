using System.Diagnostics;
using Laraue.Crawling.Abstractions;
using Microsoft.Extensions.Logging;

namespace Laraue.Crawling.Crawler;

/// <summary>
/// Base implementation of the crawler which can store it state.
/// </summary>
/// <typeparam name="TModel"></typeparam>
/// <typeparam name="TLink"></typeparam>
/// <typeparam name="TState"></typeparam>
public abstract class BaseWithStateCrawler<TModel, TLink, TState> : IWithStateCrawler<TModel, TState>
    where TModel : class
{
    private readonly ILogger<BaseWithStateCrawler<TModel, TLink, TState>> _logger;

    /// <summary>
    /// Initializes a new instance of <see cref="BaseWithStateCrawler{TModel,TLink,TState}"/>.
    /// </summary>
    /// <param name="logger"></param>
    protected BaseWithStateCrawler(ILogger<BaseWithStateCrawler<TModel, TLink, TState>> logger)
    {
        _logger = logger;
    }
    
    /// <inheritdoc />
    public IAsyncEnumerable<TModel> RunAsync(CancellationToken cancellationToken = default)
    {
        var links = GetLinksWithLogging(GetLinks());
        
        _logger.LogDebug("Links enumerator has been received");
        
        return ParsePages(links);
    }

    private async IAsyncEnumerable<TLink> GetLinksWithLogging(IAsyncEnumerable<TLink> source)
    {
        var sessionStopwatch = new Stopwatch();
        
        await foreach (var page in source)
        {
            var pageStopwatch = new Stopwatch();

            pageStopwatch.Start();
            
            _logger.LogInformation("Page {Page} processing started", page);

            yield return page;
            
            pageStopwatch.Stop();
            
            _logger.LogInformation(
                "Page {Page} processing finished for {Time}",
                page,
                pageStopwatch.Elapsed);
        }
        
        sessionStopwatch.Stop();
        
        _logger.LogDebug("Crawling session finished for {Time}", sessionStopwatch.Elapsed);
    }

    /// <inheritdoc />
    public TState? CrawlingState { get; private set; }

    /// <summary>
    /// Describes how to get a crawler state. 
    /// </summary>
    /// <returns></returns>
    protected abstract Task<TState> GetStateAsync();

    /// <inheritdoc />
    public async Task InitializeAsync()
    {
        CrawlingState = await GetStateAsync();
        
        _logger.LogInformation("State has been loaded. State: {State}", CrawlingState);
    }

    /// <summary>
    /// Describes how to save current <see cref="CrawlingState"/> to the long-term storage.
    /// </summary>
    /// <returns></returns>
    public abstract Task SaveStateAsync();
    
    /// <inheritdoc />
    public Task ResetStateAsync()
    {
        CrawlingState = default;

        return SaveStateAsync();
    }

    /// <summary>
    /// Get links to parse via a crawler.
    /// </summary>
    /// <returns></returns>
    protected abstract IAsyncEnumerable<TLink> GetLinks();
    
    /// <summary>
    /// Describes how to parse data from the passed page links.
    /// </summary>
    /// <param name="links"></param>
    /// <returns></returns>
    protected abstract IAsyncEnumerable<TModel> ParsePages(IAsyncEnumerable<TLink> links);
}