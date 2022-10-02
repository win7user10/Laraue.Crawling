namespace Laraue.Crawling.Abstractions.Schema;

public class BindObjectExpression<TElement> : BindExpression<TElement>
{
    /// <summary>
    /// Array of expressions that used to bind 
    /// </summary>
    public BindExpression<TElement>[] ChildPropertiesBinders { get; }

    public BindObjectExpression(
        Type objectType,
        SetPropertyDelegate? propertySetter,
        HtmlSelector? htmlSelector,
        BindExpression<TElement>[] childPropertiesBinders)
        : base(objectType, propertySetter, htmlSelector)
    {
        ChildPropertiesBinders = childPropertiesBinders;
    }
}