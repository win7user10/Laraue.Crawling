using System.Linq.Expressions;
using Laraue.Crawling.Abstractions;

namespace Laraue.Crawling.Dynamic;

public class DynamicHtmlSchemaBuilder<TModel, TPage> : IDynamicHtmlSchemaBuilder<TModel, TPage>
{
    private readonly List<SchemaAction> _actions = new();
    
    public IDynamicHtmlSchemaBuilder<TModel, TPage> ParseArrayProperty<TValue>(Expression<Func<TModel, TValue[]>> schemaProperty, HtmlSelector htmlSelector,
        Action<IDynamicHtmlSchemaBuilder<TValue, TPage>> childBuilder)
    {
        var property = Helper.GetParsingProperty(schemaProperty);

        var internalSchema = GetInternalSchema(childBuilder, (target, value) =>
        {
            
        });
        
        var bindingExpression = new ArrayParseExpression(
            (target, value) => property.SetValue(target, value, null),
            htmlSelector,
            typeof(TValue),
            internalSchema);
        
        _actions.Add(bindingExpression);

        return this;
    }

    public IDynamicHtmlSchemaBuilder<TModel, TPage> ParseProperty<TValue>(Expression<Func<TModel, TValue>> schemaProperty, HtmlSelector htmlSelector, Func<IHtmlElement?, TValue?> mapFunction)
    {
        return this;
    }

    public IDynamicHtmlSchemaBuilder<TModel, TPage> ParseProperty<TValue>(Expression<Func<TModel, TValue>> schemaProperty, HtmlSelector htmlSelector, Action<IDynamicHtmlSchemaBuilder<TModel, TPage>> childBuilder)
    {
        return this;
    }

    public ICompiledDynamicHtmlSchema<TModel, TPage> Build()
    {
        return new CompiledDynamicHtmlSchema<TModel, TPage>(_actions);
    }
    
    private ComplexTypeParseExpression GetInternalSchema<TValue>(Action<IDynamicHtmlSchemaBuilder<TValue, TPage>> childBuilder, Action<object, object?> propertySetter)
    {
        var internalSchemaBuilder = new DynamicHtmlSchemaBuilder<TValue, TPage>();
        
        childBuilder(internalSchemaBuilder);

        return new ComplexTypeParseExpression(
            propertySetter,
            null,
            typeof(TValue),
            internalSchemaBuilder._actions.ToArray());
    }

    public IDynamicHtmlSchemaBuilder<TModel, TPage> ExecuteAsync(Func<TPage, Task> function)
    {
        _actions.Add(new PageAction<TPage>(function));
        
        return this;
    }
}