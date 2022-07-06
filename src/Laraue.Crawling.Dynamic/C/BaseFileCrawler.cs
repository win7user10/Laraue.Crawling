using System.Text.Json;

namespace Laraue.Crawling.Dynamic.C;

public abstract class BaseFileCrawler<TModel, TState> : BaseCrawler<TModel, TState>
    where TModel : class
    where TState : class 
{
    protected abstract string StateFilePath { get; }

    protected override async ValueTask<TState?> GetInitialStateAsync()
    {
        await using var stream = File.OpenRead(StateFilePath);

        return await JsonSerializer.DeserializeAsync<TState>(stream);
    }
}