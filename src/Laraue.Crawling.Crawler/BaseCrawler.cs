using System.Diagnostics;
using Laraue.Crawling.Abstractions;
using Microsoft.Extensions.Logging;

namespace Laraue.Crawling.Crawler;

public abstract class BaseCrawler<TModel, TLink, TState> : ICrawler<TModel>
    where TModel : class
    where TState : class, new()
{
    private readonly ILogger<BaseCrawler<TModel, TLink, TState>> _logger;

    protected BaseCrawler(ILogger<BaseCrawler<TModel, TLink, TState>> logger)
    {
        _logger = logger;
    }
    
    public async Task<IAsyncEnumerable<TModel>> RunAsync(CancellationToken cancellationToken = default)
    {
        CrawlingState = await GetInitialStateAsync();
        
        _logger.LogDebug("State has been loaded. State: {State}", CrawlingState);

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
            
            _logger.LogDebug("Page {Page} processing started", page);

            yield return page;
            
            pageStopwatch.Stop();
            
            _logger.LogDebug(
                "Page {Page} processing finished for {Time}",
                page,
                pageStopwatch.Elapsed);
        }
        
        sessionStopwatch.Stop();
        
        _logger.LogDebug("Crawling session finished for {Time}", sessionStopwatch.Elapsed);
    }

    protected TState CrawlingState { get; private set; } = null!;

    protected abstract ValueTask<TState> GetInitialStateAsync();
    
    protected abstract ValueTask SaveStateAsync();

    protected abstract IAsyncEnumerable<TLink> GetLinks();
    
    protected abstract IAsyncEnumerable<TModel> ParsePages(IAsyncEnumerable<TLink> links);
}

public record BaseInitialState
{
    
}

public record LinksStatus
{
}

// 1. Load initial state
// 1.1 Load menu links parsing state
// 1.2 Load section links parsing state
// 1.3 Load pages parsing state