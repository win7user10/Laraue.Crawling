using Laraue.Core.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Laraue.Crawling.Crawler;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add crawler job to the container.
    /// </summary>
    /// <param name="services"></param>
    /// <typeparam name="TCrawlerJob">Crawler job type.</typeparam>
    /// <typeparam name="TModel">The model receiving from a crawling.</typeparam>
    /// <typeparam name="TLink">Link type that uses to open next page to parse.</typeparam>
    /// <typeparam name="TState">Class with the crawler state.</typeparam>
    /// <typeparam name="TJobRunner">Class that will run the job and manage it state.</typeparam>
    /// <returns></returns>
    public static IServiceCollection AddCrawlingService<TCrawlerJob, TModel, TLink, TState, TJobRunner>(
        this IServiceCollection services)
        where TCrawlerJob : BaseCrawlerJob<TModel, TLink, TState>
        where TModel : class
        where TState : class, new()
        where TJobRunner : JobRunner<TCrawlerJob, TState>
    {
        return services.AddBackgroundJob<TCrawlerJob, TState, TJobRunner>();
    }
}