using Laraue.Crawling.Abstractions.Schema.Delegates;

namespace Laraue.Crawling.Abstractions.Schema.Binding;

public class BindObjectExpression<TElement> : BindExpression<TElement>
{
    /// <summary>
    /// Array of expressions that used to bind 
    /// </summary>
    public SchemaExpression<TElement>[] ChildPropertiesBinders { get; }

    public BindObjectExpression(
        Type objectType,
        SetPropertyDelegate? propertySetter,
        HtmlSelector? htmlSelector,
        SchemaExpression<TElement>[] childPropertiesBinders)
        : base(objectType, propertySetter, htmlSelector)
    {
        ChildPropertiesBinders = childPropertiesBinders;
    }
}