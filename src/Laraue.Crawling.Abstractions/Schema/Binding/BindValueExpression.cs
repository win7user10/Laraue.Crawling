using Laraue.Crawling.Abstractions.Schema.Delegates;

namespace Laraue.Crawling.Abstractions.Schema.Binding;

/// <summary>
/// Represents how to parse object property of the specified type.
/// </summary>
/// <typeparam name="TElement"></typeparam>
public sealed class BindValueExpression<TElement> : BindExpression<TElement>
{
    public GetObjectValueDelegate<TElement> PropertyGetter { get; }

    public BindValueExpression(
        Type objectType,
        SetPropertyInfo? setPropertyInfo,
        HtmlSelector? htmlSelector,
        GetObjectValueDelegate<TElement> propertyGetter)
        : base(objectType, setPropertyInfo, htmlSelector)
    {
        PropertyGetter = propertyGetter;
    }
}