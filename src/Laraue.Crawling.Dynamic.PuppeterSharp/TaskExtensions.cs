namespace Laraue.Crawling.Dynamic.PuppeterSharp;

public static class TaskExtensions
{
    public static async Task<TResult> AwaitAndModify<T, TResult>(this Task<T> task, Func<T, TResult> modifyValue)
    {
        var result = await task.ConfigureAwait(false);

        return modifyValue(result);
    }
}