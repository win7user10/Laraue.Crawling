using Laraue.Crawling.Abstractions;

namespace Laraue.Crawling.Dynamic;

public record SchemaAction
{
}

public record ParseAction : SchemaAction
{
    
}

public record SimpleTypeParseExpression(
    Action<object, object?> PropertySetter,
    Func<IHtmlElement, object?> PropertyGetter,
    HtmlSelector? HtmlSelector,
    Type ObjectType)
        : ParseExpression(ObjectType, PropertySetter);

public record ArrayParseExpression(
    Action<object, object?> PropertySetter,
    HtmlSelector? HtmlSelector,
    Type ObjectType,
    ParseExpression Element)
        : ParseExpression(ObjectType, PropertySetter);

public record ComplexTypeParseExpression(
    Action<object, object?> PropertySetter,
    HtmlSelector? HtmlSelector,
    Type ObjectType,
    SchemaAction[] Actions)
    : ParseExpression(ObjectType, PropertySetter);

public abstract record ParseExpression(Type ObjectType, Action<object, object?> PropertySetter) : SchemaAction;

public record PageAction<T>(Func<T, Task> AsyncAction) : SchemaAction;