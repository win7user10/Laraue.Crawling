using System.Linq.Expressions;
using Laraue.Crawling.Abstractions;
using Laraue.Crawling.Common;

namespace Laraue.Crawling.Dynamic;

public class DynamicHtmlSchemaBuilder<TModel, TPage, TElement> : IDynamicHtmlSchemaBuilder<TModel, TPage, TElement>
{
    private readonly List<SchemaAction> _actions = new();
    
    public IDynamicHtmlSchemaBuilder<TModel, TPage, TElement> ParseArrayProperty<TValue>(Expression<Func<TModel, TValue[]>> schemaProperty, HtmlSelector htmlSelector,
        Action<IDynamicHtmlSchemaBuilder<TValue, TPage, TElement>> childBuilder)
    {
        var property = Helper.GetParsingProperty(schemaProperty);

        var internalSchema = GetInternalSchema(childBuilder, (target, value) =>
        {
            
        });
        
        var bindingExpression = new ArrayParseExpression<TElement>(
            (target, value) => property.SetValue(target, value, null),
            htmlSelector,
            typeof(TValue),
            internalSchema);
        
        _actions.Add(bindingExpression);

        return this;
    }

    public IDynamicHtmlSchemaBuilder<TModel, TPage, TElement> ParseProperty<TValue>(
        Expression<Func<TModel, TValue>> schemaProperty,
        HtmlSelector htmlSelector,
        Func<TElement?, Task<TValue?>> mapFunction)
    {
        var property = Helper.GetParsingProperty(schemaProperty);
        
        var bindingExpression = new SimpleTypeParseExpression<TElement>(
            (target, value) =>  property.SetValue(target, value, null),
            async x => await mapFunction(x).ConfigureAwait(false),
            htmlSelector,
            typeof(TValue));
        
        _actions.Add(bindingExpression);

        return this;
    }

    public IDynamicHtmlSchemaBuilder<TModel, TPage, TElement> ParseProperty<TValue>(Expression<Func<TModel, TValue>> schemaProperty, HtmlSelector htmlSelector,
        Action<IDynamicHtmlSchemaBuilder<TModel, TPage, TElement>> childBuilder)
    {
        throw new NotImplementedException();
    }

    public IDynamicHtmlSchemaBuilder<TModel, TPage, TElement> BindExactly(Func<TPage, TElement, IObjectBinder<TModel>, Task> schemaProperty)
    {
        _actions.Add(new ComplexTypeBindAction<TModel, TPage, TElement>(schemaProperty));
        
        return this;
    }

    public ICompiledDynamicHtmlSchema<TModel, TPage, TElement> Build()
    {
        return new CompiledDynamicHtmlSchema<TModel, TPage, TElement>(_actions);
    }
    
    private ComplexTypeParseExpression<TElement> GetInternalSchema<TValue>(Action<IDynamicHtmlSchemaBuilder<TValue, TPage, TElement>> childBuilder, Action<object, object?> propertySetter)
    {
        var internalSchemaBuilder = new DynamicHtmlSchemaBuilder<TValue, TPage, TElement>();
        
        childBuilder(internalSchemaBuilder);

        return new ComplexTypeParseExpression<TElement>(
            propertySetter,
            null,
            typeof(TValue),
            internalSchemaBuilder._actions.ToArray());
    }

    public IDynamicHtmlSchemaBuilder<TModel, TPage, TElement> ExecuteAsync(Func<TPage, Task> function)
    {
        _actions.Add(new PageAction<TPage>(function));
        
        return this;
    }
}