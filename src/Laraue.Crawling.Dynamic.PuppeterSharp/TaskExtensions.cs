namespace Laraue.Crawling.Dynamic.PuppeterSharp;

public static class TaskExtensions
{
    public static async Task<T> AwaitAndModify<T>(this Task<T> task, Func<T, T> modifyValue)
    {
        var result = await task.ConfigureAwait(false);

        return modifyValue(result);
    }
}