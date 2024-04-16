namespace Laraue.Crawling.Abstractions.Schema;

public class ReturnExpression<TElement, TModel> : SchemaExpression<TElement>
{
    public readonly Func<TElement, Task<TModel>> ReturnFunction;

    /// <summary>
    /// Initializes a new instance if <see cref="ReturnExpression{TElement, TModel}"/>.
    /// </summary>
    /// <param name="returnFunction"></param>
    public ReturnExpression(Func<TElement, Task<TModel>> returnFunction)
    {
        ReturnFunction = returnFunction;
    }
}