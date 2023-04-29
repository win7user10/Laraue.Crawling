using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Laraue.Crawling.Crawler.Database;

/// <summary>
/// Implementation of the <see cref="BaseWithStateCrawler{TModel,TLink,TState}"/>
/// that can stores state in the database.
/// </summary>
/// <typeparam name="TModel"></typeparam>
/// <typeparam name="TLink"></typeparam>
/// <typeparam name="TState"></typeparam>
public abstract class BaseCrawlerWithStateInDatabase<TModel, TLink, TState>
    : BaseWithStateCrawler<TModel, TLink, TState>
    where TModel : class
    where TState : class, new()
{
    private readonly string _crawlerKey;
    private readonly ICrawlerDbContext _dbContext;
    private readonly ILogger<BaseCrawlerWithStateInDatabase<TModel, TLink, TState>> _logger;
    
    /// <summary>
    /// Initializes a new instance of <see cref="BaseCrawlerWithStateInDatabase{TModel, TLink, TState}"/>
    /// </summary>
    /// <param name="crawlerKey">Unique crawler identifier.</param>
    /// <param name="dbContext">EF database context.</param>
    /// <param name="logger">Logger.</param>
    protected BaseCrawlerWithStateInDatabase(
        string crawlerKey,
        ICrawlerDbContext dbContext,
        ILogger<BaseCrawlerWithStateInDatabase<TModel, TLink, TState>> logger)
        : base(logger)
    {
        _crawlerKey = crawlerKey;
        _dbContext = dbContext;
        _logger = logger;
    }

    /// <inheritdoc />
    protected override async Task<TState> GetStateFromStorageAsync()
    {
        var state = await _dbContext
            .CrawlerState
            .FirstOrDefaultAsync(x => x.Key == _crawlerKey);

        if (state is not null)
        {
            return JsonSerializer.Deserialize<TState>(state.State) ?? new TState();
        }
        
        _logger.LogDebug("State for crawler:{Key} was not found. Start with default state", _crawlerKey);
            
        return new TState();
    }

    /// <inheritdoc />
    public override async Task SaveStateAsync()
    {
        var serializedState = JsonSerializer.Serialize(CrawlingState);
        
        await _dbContext.CrawlerState
            .Where(x => x.Key == _crawlerKey)
            .ExecuteUpdateAsync(x => x
                .SetProperty(p => p.State, serializedState));
        
        _logger.LogDebug("State {State} has been saved", CrawlingState);
    }
}