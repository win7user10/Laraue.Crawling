using System.Text.Json;

namespace Laraue.Crawling.Dynamic.C;

public abstract class BaseFileCrawler<TModel, TLink, TState> : BaseCrawler<TModel, TLink, TState>
    where TModel : class
    where TState : class, new()
{
    protected abstract string StateFilePath { get; }

    protected override async ValueTask<TState> GetInitialStateAsync()
    {
        if (!File.Exists(StateFilePath))
        {
            return new TState();
        }
        
        await using var stream = File.OpenRead(StateFilePath);

        return await JsonSerializer.DeserializeAsync<TState>(stream) ?? new TState();
    }

    protected override async ValueTask SaveStateAsync()
    {
        await using var stream = File.Open(StateFilePath, FileMode.Create);

        await stream.WriteAsync(JsonSerializer.SerializeToUtf8Bytes(CrawlingState));
    }
}