using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace Laraue.Crawling.Crawler;

/// <summary>
/// Implementation of the <see cref="BaseWithStateCrawler{TModel,TLink,TState}"/>
/// that can stores it state in the file.
/// </summary>
/// <typeparam name="TModel"></typeparam>
/// <typeparam name="TLink"></typeparam>
/// <typeparam name="TState"></typeparam>
public abstract class BaseWithFileStateCrawler<TModel, TLink, TState> : BaseWithStateCrawler<TModel, TLink, TState>
    where TModel : class
    where TState : class, new()
{
    private readonly ILogger<BaseWithFileStateCrawler<TModel, TLink, TState>> _logger;
    
    /// <summary>
    /// Initializes a new instance of <see cref="BaseWithFileStateCrawler{TModel,TLink,TState}"/>.
    /// </summary>
    /// <param name="logger"></param>
    protected BaseWithFileStateCrawler(ILogger<BaseWithFileStateCrawler<TModel, TLink, TState>> logger)
        : base(logger)
    {
        _logger = logger;
    }
    
    /// <summary>
    /// Gets the path where the current crawling state should be stored.
    /// </summary>
    protected abstract string StateFilePath { get; }

    /// <inheritdoc />
    protected override async Task<TState> GetStateFromStorageAsync()
    {
        if (!File.Exists(StateFilePath))
        {
            _logger.LogDebug("State file {File} was not found. Start with default state", StateFilePath);
            
            return new TState();
        }
        
        await using var stream = File.OpenRead(StateFilePath);

        return await JsonSerializer.DeserializeAsync<TState>(stream) ?? new TState();
    }

    /// <inheritdoc />
    public override async Task SaveStateAsync()
    {
        await using var stream = File.Open(StateFilePath, FileMode.Create);

        await stream.WriteAsync(JsonSerializer.SerializeToUtf8Bytes(CrawlingState));
        
        _logger.LogDebug("State {State} has been saved", CrawlingState);
    }
}