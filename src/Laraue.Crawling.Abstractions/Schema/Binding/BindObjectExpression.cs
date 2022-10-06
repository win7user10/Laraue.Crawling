namespace Laraue.Crawling.Abstractions.Schema.Binding;

public class BindObjectExpression<TElement> : BindExpression<TElement>
{
    /// <summary>
    /// Array of expressions that used to bind 
    /// </summary>
    public SchemaExpression<TElement>[] ChildPropertiesBinders { get; }

    public BindObjectExpression(
        Type objectType,
        SetPropertyInfo? setPropertyInfo,
        HtmlSelector? htmlSelector,
        SchemaExpression<TElement>[] childPropertiesBinders)
        : base(objectType, setPropertyInfo, htmlSelector)
    {
        ChildPropertiesBinders = childPropertiesBinders;
    }
}