using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace Laraue.Crawling.Crawler;

public abstract class BaseFileCrawler<TModel, TLink, TState> : BaseCrawler<TModel, TLink, TState>
    where TModel : class
    where TState : class, new()
{
    private readonly ILogger<BaseFileCrawler<TModel, TLink, TState>> _logger;
    
    protected BaseFileCrawler(ILogger<BaseFileCrawler<TModel, TLink, TState>> logger)
        : base(logger)
    {
        _logger = logger;
    }
    
    protected abstract string StateFilePath { get; }

    protected override async ValueTask<TState> GetInitialStateAsync()
    {
        if (!File.Exists(StateFilePath))
        {
            _logger.LogDebug("State file {File} was not found. Start with default state", StateFilePath);
            
            return new TState();
        }
        
        await using var stream = File.OpenRead(StateFilePath);

        return await JsonSerializer.DeserializeAsync<TState>(stream) ?? new TState();
    }

    protected override async ValueTask SaveStateAsync()
    {
        await using var stream = File.Open(StateFilePath, FileMode.Create);

        await stream.WriteAsync(JsonSerializer.SerializeToUtf8Bytes(CrawlingState));
        
        _logger.LogDebug("State {State} has been saved", CrawlingState);
    }
}