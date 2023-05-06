using Microsoft.EntityFrameworkCore;

namespace Laraue.Crawling.Crawler.Database;

/// <summary>
/// <see cref="DbContext"/> that have a table for crawlers states.
/// </summary>
public interface ICrawlerDbContext
{
    /// <summary>
    /// All crawlers states.
    /// </summary>
    public DbSet<CrawlerStateEntity> CrawlerState { get; init; }

    /// <summary>
    /// Save changes method.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}