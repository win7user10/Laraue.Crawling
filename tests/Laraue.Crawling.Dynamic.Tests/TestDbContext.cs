using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Laraue.Crawling.Dynamic.Tests;

public class TestDbContext : DbContext
{
    public TestDbContext(DbContextOptions<TestDbContext> options)
        : base(options)
    {
    }
    
    public DbSet<CianPage> CianPages { get; init; }
}

public class ContextFactory : IDesignTimeDbContextFactory<TestDbContext>
{
    public TestDbContext CreateDbContext(string[] args)
    {
        return new TestDbContext(
            new DbContextOptionsBuilder<TestDbContext>()
                .UseNpgsql("User ID=postgres;Password=postgres;Host=localhost;Port=5432;Database=cian_pages;")
                .Options);
    }
}