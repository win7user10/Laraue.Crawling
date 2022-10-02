namespace Laraue.Crawling.Abstractions.Schema;

public class ActionExpression<TElement> : SchemaExpression<TElement>
{
    public readonly Func<TElement, Task> AsyncAction;

    public ActionExpression(Func<TElement, Task> asyncAction)
    {
        AsyncAction = asyncAction;
    }
}