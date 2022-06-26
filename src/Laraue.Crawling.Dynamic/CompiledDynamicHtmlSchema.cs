namespace Laraue.Crawling.Dynamic;

public class CompiledDynamicHtmlSchema<TModel, TPage, TElement> : ICompiledDynamicHtmlSchema<TModel, TPage, TElement>
{
    public CompiledDynamicHtmlSchema(IEnumerable<SchemaAction> actions)
    {
        Actions = actions.ToArray();
        ObjectType = typeof(TModel);
    }
    
    public SchemaAction[] Actions { get; }
    public Type ObjectType { get; }
}