using Laraue.Crawling.Abstractions;

namespace Laraue.Crawling.Dynamic;

public record SchemaAction
{
}

public record ParseAction : SchemaAction
{
    
}

public record SimpleTypeParseExpression<TElement>(
    Action<object, object?> PropertySetter,
    Func<TElement, Task<object?>> AsyncPropertyGetter,
    HtmlSelector? HtmlSelector,
    Type ObjectType)
        : ParseExpression<TElement>(ObjectType, PropertySetter);

public record ArrayParseExpression<TElement>(
    Action<object, object?> PropertySetter,
    HtmlSelector? HtmlSelector,
    Type ObjectType,
    ParseExpression<TElement> Element)
        : ParseExpression<TElement>(ObjectType, PropertySetter);

public record ComplexTypeParseExpression<TElement>(
    Action<object, object?> PropertySetter,
    HtmlSelector? HtmlSelector,
    Type ObjectType,
    SchemaAction[] Actions)
    : ParseExpression<TElement>(ObjectType, PropertySetter), IObjectElement;

public abstract record ParseExpression<TElement>(Type ObjectType, Action<object, object?> PropertySetter) : SchemaAction;

public record PageAction<T>(Func<T, Task> AsyncAction) : SchemaAction;