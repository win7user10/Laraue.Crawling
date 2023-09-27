namespace Laraue.Crawling.Abstractions.Schema.Binding;

/// <summary>
/// Represents how to parse array property of the specified type.
/// </summary>
public sealed class BindArrayExpression<TElement, TSelector> : BindExpression<TElement, TSelector>
    where TSelector : Selector
{
    public BindExpression<TElement, TSelector> Element { get; }
    
    public BindArrayExpression(
        Type objectType,
        SetPropertyInfo? setPropertyInfo,
        TSelector? selector,
        BindExpression<TElement, TSelector> element)
        : base(objectType, setPropertyInfo, selector)
    {
        Element = element;
    }
}