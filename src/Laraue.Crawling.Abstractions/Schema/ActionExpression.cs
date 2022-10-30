namespace Laraue.Crawling.Abstractions.Schema;

/// <summary>
/// Represents async delegate that should be performed.
/// </summary>
/// <typeparam name="TElement"></typeparam>
public class ActionExpression<TElement> : SchemaExpression<TElement>
{
    /// <summary>
    /// Async delegate to run.
    /// </summary>
    public readonly Func<TElement, Task> AsyncAction;

    
    /// <summary>
    /// Initializes a new instance if <see cref="ActionExpression{TElement}"/>.
    /// </summary>
    /// <param name="asyncAction"></param>
    public ActionExpression(Func<TElement, Task> asyncAction)
    {
        AsyncAction = asyncAction;
    }
}