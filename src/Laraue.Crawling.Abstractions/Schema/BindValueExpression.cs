namespace Laraue.Crawling.Abstractions.Schema;

/// <summary>
/// Represents how to parse object property of the specified type.
/// </summary>
/// <typeparam name="TElement"></typeparam>
public sealed class BindValueExpression<TElement> : BindExpression<TElement>
{
    public GetObjectValueDelegate<TElement> PropertyGetter { get; }

    public BindValueExpression(
        Type objectType,
        SetPropertyDelegate? propertySetter,
        HtmlSelector? htmlSelector,
        GetObjectValueDelegate<TElement> propertyGetter)
        : base(objectType, propertySetter, htmlSelector)
    {
        PropertyGetter = propertyGetter;
    }
}