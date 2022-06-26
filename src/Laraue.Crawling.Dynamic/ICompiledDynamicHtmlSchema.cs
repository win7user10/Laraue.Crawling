namespace Laraue.Crawling.Dynamic;

public interface ICompiledDynamicHtmlSchema<TModel, TPage, TElement> : IObjectElement
{
}

public interface IObjectElement
{
    SchemaAction[] Actions { get; }
    Type ObjectType { get; }
}