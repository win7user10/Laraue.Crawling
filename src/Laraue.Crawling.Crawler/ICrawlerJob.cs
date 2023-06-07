using Laraue.Core.Extensions.Hosting;
using Laraue.Crawling.Abstractions;

namespace Laraue.Crawling.Crawler;

/// <summary>
/// The <see cref="ICrawler{TState}"/> that contains a state that can be stored somehow.
/// </summary>
/// <typeparam name="TState"></typeparam>
public interface ICrawlerJob<TState> : IJob<TState>
    where TState : class, new()
{
}