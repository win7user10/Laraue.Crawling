using System.Linq.Expressions;
using Laraue.Crawling.Abstractions;
using Laraue.Crawling.Abstractions.Schema;
using Laraue.Crawling.Abstractions.Schema.Binding;
using Laraue.Crawling.Abstractions.Schema.Delegates;

namespace Laraue.Crawling.Common.Impl;

/// <summary>
/// Builder for the static html schema. "Static" means static html,
/// it will not use a browser for the crawling.
/// </summary>
/// <typeparam name="TElement"></typeparam>
/// <typeparam name="TModel"></typeparam>
public class HtmlSchemaBuilder<TElement, TModel>
{
    public readonly List<SchemaExpression<TElement>> BindingExpressions = new ();
    
    public HtmlSchemaBuilder<TElement, TModel> HasProperty<TValue>(
        Expression<Func<TModel, TValue?>> schemaProperty,
        HtmlSelector htmlSelector,
        GetValueDelegate<TElement, TValue> getValueDelegate)
    {
        var property = Helper.GetParsingProperty(schemaProperty);
        
        var bindingExpression = new BindValueExpression<TElement>(
            typeof(TValue),
            new SetPropertyInfo(property),
            htmlSelector,
            async element => await getValueDelegate.Invoke(element).ConfigureAwait(false));
        
        BindingExpressions.Add(bindingExpression);

        return this;
    }

    public HtmlSchemaBuilder<TElement, TModel> HasObjectProperty<TValue>(
        Expression<Func<TModel, TValue?>> schemaProperty,
        HtmlSelector htmlSelector,
        Action<HtmlSchemaBuilder<TElement, TValue>> childBuilder)
    {
        var property = Helper.GetParsingProperty(schemaProperty);
        
        var internalSchema = GetInternalSchema(
            childBuilder,
            new SetPropertyInfo(property));
        
        var bindingExpression = new BindObjectExpression<TElement>(
            typeof(TValue),
            new SetPropertyInfo(property),
            htmlSelector,
            internalSchema.ChildPropertiesBinders);
        
        BindingExpressions.Add(bindingExpression);

        return this;
    }

    public HtmlSchemaBuilder<TElement, TModel> HasArrayProperty<TValue>(
        Expression<Func<TModel, TValue[]?>> schemaProperty,
        HtmlSelector htmlSelector,
        GetValueDelegate<TElement?, TValue> mapFunction)
    {
        var property = Helper.GetParsingProperty(schemaProperty);
        
        var bindingExpression = new BindArrayExpression<TElement>(
            typeof(TValue),
            new SetPropertyInfo(property),
            htmlSelector,
            new BindValueExpression<TElement>(
                typeof(TValue),
                new SetPropertyInfo(property),
                null,
                async element => await mapFunction.Invoke(element).ConfigureAwait(false)));
        
        BindingExpressions.Add(bindingExpression);

        return this;
    }

    public HtmlSchemaBuilder<TElement, TModel> HasArrayProperty<TValue>(
        Expression<Func<TModel, TValue[]?>> schemaProperty,
        HtmlSelector htmlSelector,
        Action<HtmlSchemaBuilder<TElement, TValue>> childBuilder)
    {
        var property = Helper.GetParsingProperty(schemaProperty);

        var internalSchema = GetInternalSchema(childBuilder);
        
        var bindingExpression = new BindArrayExpression<TElement>(
            typeof(TValue),
            new SetPropertyInfo(property),
            htmlSelector,
            internalSchema);
        
        BindingExpressions.Add(bindingExpression);

        return this;
    }
    
    private BindObjectExpression<TElement> GetInternalSchema<TValue>(
        Action<HtmlSchemaBuilder<TElement, TValue>> childBuilder,
        SetPropertyInfo? setPropertyInfo = null)
    {
        var internalSchemaBuilder = new HtmlSchemaBuilder<TElement, TValue>();
        
        childBuilder(internalSchemaBuilder);

        return new BindObjectExpression<TElement>(
            typeof(TValue),
            setPropertyInfo,
            null,
            internalSchemaBuilder.BindingExpressions.ToArray());
    }
    
    public HtmlSchemaBuilder<TElement, TModel> BindManually(Func<TElement, IObjectBinder<TModel>, Task> schemaProperty)
    {
        BindingExpressions.Add(new ManualBindExpression<TElement, TModel>(schemaProperty));
        
        return this;
    }
    
    public HtmlSchemaBuilder<TElement, TModel> ExecuteAsync(Func<TElement, Task> function)
    {
        BindingExpressions.Add(new ActionExpression<TElement>(function));
        
        return this;
    }

    public ICompiledHtmlSchema<TElement, TModel> Build()
    {
        return new CompiledHtmlSchema<TElement, TModel>(
            new BindObjectExpression<TElement>(
                typeof(TModel),
                null,
                null,
                BindingExpressions.ToArray()));
    }
}