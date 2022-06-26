using Laraue.Crawling.Abstractions;

namespace Laraue.Crawling.Static.Abstractions;

public abstract record BindingExpression(Type ObjectType, Action<object, object?> PropertySetter);

/// <summary>
/// 
/// </summary>
/// <param name="PropertySetter">Build to the passed object passed property.</param>
/// <param name="PropertyGetter">Get the final value from the passed html.</param>
/// <param name="HtmlSelector"></param>
/// <param name="ObjectType"></param>
public record SimpleTypeBindingExpression(
    Action<object, object?> PropertySetter,
    Func<IHtmlElement, object?> PropertyGetter,
    HtmlSelector? HtmlSelector,
    Type ObjectType)
    : BindingExpression(ObjectType, PropertySetter);

public record ArrayBindingExpression(
    Action<object, object?> PropertySetter,
    HtmlSelector? HtmlSelector,
    Type ObjectType,
    BindingExpression Element)
    : BindingExpression(ObjectType, PropertySetter);
    
public record ComplexTypeBindingExpression(
    Action<object, object?> PropertySetter,
    HtmlSelector? HtmlSelector,
    Type ObjectType,
    BindingExpression[] Elements)
    : BindingExpression(ObjectType, PropertySetter);