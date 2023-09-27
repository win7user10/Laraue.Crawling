using Laraue.Crawling.Abstractions.Schema.Delegates;

namespace Laraue.Crawling.Abstractions.Schema.Binding;

/// <summary>
/// Represents how to parse object property of the specified type.
/// </summary>
/// <typeparam name="TElement"></typeparam>
/// <typeparam name="TSelector"></typeparam>
public sealed class BindValueExpression<TElement, TSelector> : BindExpression<TElement, TSelector>
    where TSelector : Selector
{
    public GetObjectValueDelegate<TElement> PropertyGetter { get; }

    public BindValueExpression(
        Type objectType,
        SetPropertyInfo? setPropertyInfo,
        TSelector? selector,
        GetObjectValueDelegate<TElement> propertyGetter)
        : base(objectType, setPropertyInfo, selector)
    {
        PropertyGetter = propertyGetter;
    }
}