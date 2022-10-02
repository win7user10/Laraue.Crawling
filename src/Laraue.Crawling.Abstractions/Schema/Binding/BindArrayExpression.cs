using Laraue.Crawling.Abstractions.Schema.Delegates;

namespace Laraue.Crawling.Abstractions.Schema.Binding;

/// <summary>
/// Represents how to parse array property of the specified type.
/// </summary>
public sealed class BindArrayExpression<TElement> : BindExpression<TElement>
{
    public BindExpression<TElement> Element { get; }
    
    public BindArrayExpression(
        Type objectType,
        SetPropertyDelegate? propertySetter,
        HtmlSelector? htmlSelector,
        BindExpression<TElement> element)
        : base(objectType, propertySetter, htmlSelector)
    {
        Element = element;
    }
}