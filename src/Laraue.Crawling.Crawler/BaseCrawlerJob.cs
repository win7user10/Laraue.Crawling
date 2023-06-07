using System.Diagnostics;
using Laraue.Core.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Laraue.Crawling.Crawler;

/// <summary>
/// Base implementation of the crawler which can store it state.
/// </summary>
/// <typeparam name="TModel"></typeparam>
/// <typeparam name="TLink"></typeparam>
/// <typeparam name="TState"></typeparam>
public abstract class BaseCrawlerJob<TModel, TLink, TState> : ICrawlerJob<TState>
    where TModel : class
    where TState : class, new()
{
    private readonly ILogger<BaseCrawlerJob<TModel, TLink, TState>> _logger;

    /// <summary>
    /// Initializes a new instance of <see cref="BaseCrawlerJob{TModel,TLink,TState}"/>.
    /// </summary>
    /// <param name="logger"></param>
    protected BaseCrawlerJob(ILogger<BaseCrawlerJob<TModel, TLink, TState>> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<TimeSpan> ExecuteAsync(JobState<TState> jobState, CancellationToken stoppingToken)
    {
        var sessionStopwatch = new Stopwatch();
        sessionStopwatch.Start();
        
        var pageStopwatch = new Stopwatch();
        pageStopwatch.Start();
            
        var link = await GetNextLinkAsync(stoppingToken).ConfigureAwait(false);
        if (link == null)
        {
            _logger.LogInformation("Crawling session finished for {Time}", sessionStopwatch.Elapsed);
            return GetTimeToWait();
        }
            
        _logger.LogInformation("Page {Page} processing started", link);
            
        await ParseLinkAsync(link, stoppingToken).ConfigureAwait(false);
        await UpdateStateAsync(jobState, stoppingToken).ConfigureAwait(false);
            
        _logger.LogInformation(
            "Page {Page} processing finished for {Time}",
            link,
            pageStopwatch.Elapsed);
        
        return TimeSpan.Zero;
    }

    /// <summary>
    /// Return next link should be parsed or null to pause.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    protected abstract Task<TLink?> GetNextLinkAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Execute something after one link has been parsed. Here 
    /// </summary>
    /// <param name="state"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    protected abstract Task UpdateStateAsync(JobState<TState> state, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// The body of parsing.
    /// </summary>
    /// <param name="link"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    protected abstract Task<TModel?> ParseLinkAsync(TLink link, CancellationToken cancellationToken = default);

    /// <summary>
    /// Return how long to wait before the next crawling session.
    /// </summary>
    /// <returns></returns>
    protected abstract TimeSpan GetTimeToWait();
}