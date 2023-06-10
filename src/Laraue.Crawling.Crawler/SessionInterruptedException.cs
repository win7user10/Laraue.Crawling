namespace Laraue.Crawling.Crawler;

/// <summary>
/// Exception that allow to interrupt crawling process and finish the session.
/// </summary>
public sealed class SessionInterruptedException : Exception
{
    /// <summary>
    /// Initializes a new instance of <see cref="SessionInterruptedException"/> with description of interrupting.
    /// </summary>
    /// <param name="message"></param>
    public SessionInterruptedException(string message) : base(message)
    {}
}