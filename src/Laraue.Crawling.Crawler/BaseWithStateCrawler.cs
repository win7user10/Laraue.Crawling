using System.Diagnostics;
using System.Runtime.CompilerServices;
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
    where TState : class, new()
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
        var links = GetLinksWithLogging(GetLinks(cancellationToken), cancellationToken);
        
        _logger.LogDebug("Links enumerator has been received");
        
        return ParsePages(links, cancellationToken);
    }

    private async IAsyncEnumerable<TLink> GetLinksWithLogging(
        IAsyncEnumerable<TLink> source,
        [EnumeratorCancellation] CancellationToken ct)
    {
        var sessionStopwatch = new Stopwatch();
        
        await foreach (var page in source.WithCancellation(ct))
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

            await SaveStateAsync();
            
            _logger.LogInformation("Crawler state is updated to {State}", CrawlingState);
        }
        
        sessionStopwatch.Stop();
        
        _logger.LogDebug("Crawling session finished for {Time}", sessionStopwatch.Elapsed);
    }

    /// <inheritdoc />
    public TState? CrawlingState { get; private set; }

    /// <summary>
    /// Gets the state that has been stored for this crawler.
    /// Note - the actual crawler state can be not stored in the storage and be different.
    /// To sync it use <see cref="SaveStateAsync"/>. 
    /// </summary>
    /// <returns></returns>
    protected abstract Task<TState> GetStateFromStorageAsync();

    /// <inheritdoc />
    public async Task InitializeAsync()
    {
        CrawlingState = await GetStateFromStorageAsync();
        
        _logger.LogInformation("State has been loaded. State: {State}", CrawlingState);
    }

    /// <summary>
    /// Store current in memory <see cref="CrawlingState"/> to the long-term storage.
    /// </summary>
    /// <returns></returns>
    public abstract Task SaveStateAsync();
    
    /// <inheritdoc />
    public Task ResetStateAsync()
    {
        CrawlingState = new TState();

        return SaveStateAsync();
    }

    /// <summary>
    /// Get links to parse via a crawler.
    /// </summary>
    /// <returns></returns>
    protected abstract IAsyncEnumerable<TLink> GetLinks(CancellationToken ct = default);
    
    /// <summary>
    /// Describes how to parse data from the passed page links.
    /// </summary>
    /// <param name="links"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    protected abstract IAsyncEnumerable<TModel> ParsePages(
        IAsyncEnumerable<TLink> links,
        CancellationToken ct = default);
}