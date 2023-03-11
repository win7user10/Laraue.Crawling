namespace Laraue.Crawling.Dynamic.PuppeterSharp;

/// <summary>
/// Extension for the tasks returning during the different elements crawling. 
/// </summary>
public static class TaskExtensions
{
    /// <summary>
    /// Await the passed task, then apply a modify function, then return the final result.
    /// </summary>
    /// <param name="task"></param>
    /// <param name="modifyValue"></param>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <returns></returns>
    public static async Task<TResult> AwaitAndModify<T, TResult>(this Task<T> task, Func<T, TResult> modifyValue)
    {
        var result = await task.ConfigureAwait(false);

        return modifyValue(result);
    }
    
    /// <summary>
    /// Await the passed task, then apply an async modify function, then return the final result.
    /// </summary>
    /// <param name="task"></param>
    /// <param name="modifyValue"></param>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <returns></returns>
    public static async Task<TResult> AwaitAndModify<T, TResult>(this Task<T> task, Func<T, Task<TResult>> modifyValue)
    {
        var result = await task.ConfigureAwait(false);

        var finalResult = await modifyValue(result).ConfigureAwait(false);

        return finalResult;
    }
}