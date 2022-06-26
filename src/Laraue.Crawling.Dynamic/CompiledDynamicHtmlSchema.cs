namespace Laraue.Crawling.Dynamic;

public class CompiledDynamicHtmlSchema<TModel, TPage> : ICompiledDynamicHtmlSchema<TModel, TPage>
{
    public CompiledDynamicHtmlSchema(IEnumerable<SchemaAction> actions)
    {
        Actions = actions.ToArray();
    }
    
    public SchemaAction[] Actions { get; }
}