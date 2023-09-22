namespace Laraue.Crawling.Crawler;

/// <summary>
/// Exception that describe to the system that crawler has been detected and the passed
/// delegate should be executed to continue the crawling process.
/// </summary>
public sealed class CrawlerHasBeenDetectedException : Exception
{
    /// <summary>
    /// Delegate to return a crawler to the normal state.
    /// </summary>
    public Func<Task> SwitchToCorrectStateAsync { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="SessionInterruptedException"/> with description of interrupting.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="switchToCorrectStateAsync"></param>
    public CrawlerHasBeenDetectedException(string message, Func<Task> switchToCorrectStateAsync) : base(message)
    {
        SwitchToCorrectStateAsync = switchToCorrectStateAsync;
    }
}