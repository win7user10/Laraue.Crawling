using Laraue.Crawling.Abstractions.Schema.Delegates;

namespace Laraue.Crawling.Abstractions.Schema;

public class ReturnExpression<TElement, TSelector> : SchemaExpression<TElement>
    where TSelector : Selector
{
    public TSelector? Selector { get; set; }
    public GetObjectValueDelegate<TElement> ReturnFunction;

    public ReturnExpression(GetObjectValueDelegate<TElement> returnFunction, TSelector? selector)
    {
        Selector = selector;
        ReturnFunction = returnFunction;
    }
}