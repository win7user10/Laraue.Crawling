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
public abstract class BaseCrawlerJob<TModel, TLink, TState> : BaseJob<TState>
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
    public override async Task<TimeSpan> ExecuteAsync(JobState<TState> jobState, CancellationToken stoppingToken)
    {
        await OnSessionStartAsync(jobState, stoppingToken).ConfigureAwait(false);
        
        var pageStopwatch = new Stopwatch();
        pageStopwatch.Start();

        while (true)
        {
            var link = await GetNextLinkAsync(jobState, stoppingToken).ConfigureAwait(false);
            if (link == null)
            {
                return await RunSessionFinishAsync(jobState, stoppingToken).ConfigureAwait(false);
            }
            
            _logger.LogInformation("Page {Page} processing started", link);
            
            var result = await ParseLinkAsync(link, jobState, stoppingToken).ConfigureAwait(false);
            if (result is null)
            {
                return await RunSessionFinishAsync(jobState, stoppingToken).ConfigureAwait(false);
            }
            
            await AfterLinkParsedAsync(link, result, jobState, stoppingToken).ConfigureAwait(false);
            
            _logger.LogInformation(
                "Page {Page} processing finished for {Time}",
                link,
                pageStopwatch.Elapsed);
        
            stoppingToken.ThrowIfCancellationRequested();
        }
    }

    private async Task<TimeSpan> RunSessionFinishAsync(JobState<TState> jobState, CancellationToken stoppingToken = default)
    {
        await OnSessionFinishAsync(jobState, stoppingToken).ConfigureAwait(false);
        
        return GetTimeToWait();
    }

    /// <summary>
    /// Return next link should be parsed or null to pause.
    /// </summary>
    /// <param name="state"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    protected abstract Task<TLink?> GetNextLinkAsync(JobState<TState> state, CancellationToken cancellationToken = default);

    /// <summary>
    /// The body of parsing.
    /// </summary>
    /// <param name="link"></param>
    /// <param name="state"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    protected abstract Task<TModel?> ParseLinkAsync(TLink link, JobState<TState> state, CancellationToken cancellationToken = default);

    /// <summary>
    /// Return how long to wait before the next crawling session.
    /// </summary>
    /// <returns></returns>
    protected abstract TimeSpan GetTimeToWait();

    /// <summary>
    /// Do something when crawling session finished.
    /// </summary>
    /// <param name="state"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    protected abstract Task OnSessionStartAsync(JobState<TState> state, CancellationToken cancellationToken = default);

    /// <summary>
    /// Do something when crawling session started.
    /// </summary>
    /// <param name="state"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    protected abstract Task OnSessionFinishAsync(JobState<TState> state, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Execute something after one link has been parsed. 
    /// </summary>
    /// <param name="link"></param>
    /// <param name="model"></param>
    /// <param name="state"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    protected abstract Task AfterLinkParsedAsync(TLink link, TModel model, JobState<TState> state, CancellationToken cancellationToken = default);
}