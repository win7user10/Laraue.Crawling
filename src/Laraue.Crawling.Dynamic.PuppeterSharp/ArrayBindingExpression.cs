using Laraue.Crawling.Abstractions;

namespace Laraue.Crawling.Dynamic.PuppeterSharp;

public record ArrayBindingExpression(
    Action<object, object?> PropertySetter,
    HtmlSelector? HtmlSelector,
    Type ObjectType);