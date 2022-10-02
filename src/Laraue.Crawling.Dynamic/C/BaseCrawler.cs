using Laraue.Crawling.Abstractions;

namespace Laraue.Crawling.Dynamic.C;

public abstract class BaseCrawler<TModel, TLink, TState> : ICrawler<TModel>
    where TModel : class
    where TState : class, new()
{
    protected TState CrawlingState { get; private set; } = null!;

    protected abstract ValueTask<TState> GetInitialStateAsync();
    
    protected abstract ValueTask SaveStateAsync();
    
    public async Task<IAsyncEnumerable<TModel>> RunAsync(CancellationToken cancellationToken = default)
    {
        CrawlingState = await GetInitialStateAsync();

        var links = GetLinks();
        
        return ParsePages(links);
    }

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