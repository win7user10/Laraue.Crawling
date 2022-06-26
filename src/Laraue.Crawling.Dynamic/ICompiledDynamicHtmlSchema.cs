namespace Laraue.Crawling.Dynamic;

public interface ICompiledDynamicHtmlSchema<TModel, TPage>
{
    SchemaAction[] Actions { get; }
}