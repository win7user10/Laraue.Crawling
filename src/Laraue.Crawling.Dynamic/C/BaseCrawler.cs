using Laraue.Crawling.Abstractions;

namespace Laraue.Crawling.Dynamic.C;

public abstract class BaseCrawler<TModel, TState> : ICrawler<TModel>
    where TModel : class
{
    protected TState? CrawlingState { get; private set; }

    protected abstract ValueTask<TState?> GetInitialStateAsync();
    
    public async Task<IAsyncEnumerable<TModel>> RunAsync()
    {
        CrawlingState = await GetInitialStateAsync();
        
        await ParseSectionLinksAsync();
        await ParsePageLinksAsync();
        
        return ParsePages();
    }

    protected abstract Task ParseSectionLinksAsync();
    
    protected abstract Task ParsePageLinksAsync();
    
    protected abstract IAsyncEnumerable<TModel> ParsePages();
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