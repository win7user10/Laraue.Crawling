namespace Laraue.Crawling.Abstractions.Schema.Binding;

public class ManualBindExpression<TElement, TModel> : ManualBindExpression<TElement>
{
    public ManualBindExpression(Func<TElement, IObjectBinder<TModel>, Task> asyncBindFunction)
        : base(asyncBindFunction)
    {
    }
}

public class ManualBindExpression<TElement> : SchemaExpression<TElement>
{
    public readonly Delegate AsyncBindFunction;

    internal ManualBindExpression(Delegate asyncBindFunction)
    {
        AsyncBindFunction = asyncBindFunction;
    }
}