using Laraue.Core.Extensions.Hosting.EfCore;
using Microsoft.Extensions.DependencyInjection;

namespace Laraue.Crawling.Crawler.EfCore;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add crawler job to the container.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="jobKey"></param>
    /// <typeparam name="TCrawlerJob">Crawler job type.</typeparam>
    /// <typeparam name="TModel">The model receiving from a crawling.</typeparam>
    /// <typeparam name="TLink">Link type that uses to open next page to parse.</typeparam>
    /// <typeparam name="TState">Class with the crawler state.</typeparam>
    /// <returns></returns>
    public static IServiceCollection AddCrawlingService<TCrawlerJob, TModel, TLink, TState>(
        this IServiceCollection services,
        string jobKey)
        where TCrawlerJob : BaseCrawlerJob<TModel, TLink, TState>
        where TModel : class
        where TState : class, new()
    {
        return services.AddCrawlingService<TCrawlerJob, TModel, TLink, TState, DbJobRunner<TCrawlerJob, TState>>(jobKey);
    }
}