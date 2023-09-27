using System.Linq.Expressions;
using Laraue.Crawling.Abstractions;
using Laraue.Crawling.Abstractions.Schema;
using Laraue.Crawling.Abstractions.Schema.Binding;
using Laraue.Crawling.Abstractions.Schema.Delegates;

namespace Laraue.Crawling.Common.Impl;

/// <summary>
/// Builder for the static schema. "Static" means static document,
/// which is just a text that will no change.
/// </summary>
/// <typeparam name="TElement"></typeparam>
/// <typeparam name="TSelector"></typeparam>
/// <typeparam name="TModel"></typeparam>
public class DocumentSchemaBuilder<TElement, TSelector, TModel>
    where TSelector : Selector
{
    public readonly List<SchemaExpression<TElement>> BindingExpressions = new ();
    
    public DocumentSchemaBuilder<TElement, TSelector, TModel> HasProperty<TValue>(
        Expression<Func<TModel, TValue?>> schemaProperty,
        TSelector? htmlSelector,
        GetValueDelegate<TElement, TValue> getValueDelegate)
    {
        var property = Helper.GetParsingProperty(schemaProperty);
        
        var bindingExpression = new BindValueExpression<TElement, TSelector>(
            typeof(TValue),
            new SetPropertyInfo(property),
            htmlSelector,
            async element => await getValueDelegate.Invoke(element).ConfigureAwait(false));
        
        BindingExpressions.Add(bindingExpression);

        return this;
    }

    public DocumentSchemaBuilder<TElement, TSelector, TModel> HasObjectProperty<TValue>(
        Expression<Func<TModel, TValue?>> schemaProperty,
        TSelector? htmlSelector,
        Action<DocumentSchemaBuilder<TElement, TSelector, TValue>> childBuilder)
    {
        var property = Helper.GetParsingProperty(schemaProperty);
        
        var internalSchema = GetInternalSchema(
            childBuilder,
            new SetPropertyInfo(property));
        
        var bindingExpression = new BindObjectExpression<TElement, TSelector>(
            typeof(TValue),
            new SetPropertyInfo(property),
            htmlSelector,
            internalSchema.ChildPropertiesBinders);
        
        BindingExpressions.Add(bindingExpression);

        return this;
    }

    public DocumentSchemaBuilder<TElement, TSelector, TModel> HasArrayProperty<TValue>(
        Expression<Func<TModel, TValue[]?>> schemaProperty,
        TSelector? selector,
        GetValueDelegate<TElement?, TValue> mapFunction)
    {
        var property = Helper.GetParsingProperty(schemaProperty);
        
        var bindingExpression = new BindArrayExpression<TElement, TSelector>(
            typeof(TValue),
            new SetPropertyInfo(property),
            selector,
            new BindValueExpression<TElement, TSelector>(
                typeof(TValue),
                new SetPropertyInfo(property),
                null,
                async element => await mapFunction.Invoke(element).ConfigureAwait(false)));
        
        BindingExpressions.Add(bindingExpression);

        return this;
    }

    public DocumentSchemaBuilder<TElement, TSelector, TModel> HasArrayProperty<TValue>(
        Expression<Func<TModel, IEnumerable<TValue>?>> schemaProperty,
        TSelector? selector,
        Action<DocumentSchemaBuilder<TElement, TSelector, TValue>> childBuilder)
    {
        var property = Helper.GetParsingProperty(schemaProperty);

        var internalSchema = GetInternalSchema(childBuilder);
        
        var bindingExpression = new BindArrayExpression<TElement, TSelector>(
            typeof(TValue),
            new SetPropertyInfo(property),
            selector,
            internalSchema);
        
        BindingExpressions.Add(bindingExpression);

        return this;
    }
    
    private BindObjectExpression<TElement, TSelector> GetInternalSchema<TValue>(
        Action<DocumentSchemaBuilder<TElement, TSelector, TValue>> childBuilder,
        SetPropertyInfo? setPropertyInfo = null)
    {
        var internalSchemaBuilder = new DocumentSchemaBuilder<TElement, TSelector, TValue>();
        
        childBuilder(internalSchemaBuilder);

        return new BindObjectExpression<TElement, TSelector>(
            typeof(TValue),
            setPropertyInfo,
            null,
            internalSchemaBuilder.BindingExpressions.ToArray());
    }
    
    public DocumentSchemaBuilder<TElement, TSelector, TModel> BindManually(Func<TElement, IObjectBinder<TModel>, Task> schemaProperty)
    {
        BindingExpressions.Add(new ManualBindExpression<TElement, TModel>(schemaProperty));
        
        return this;
    }
    
    public DocumentSchemaBuilder<TElement, TSelector, TModel> ExecuteAsync(Func<TElement, Task> function)
    {
        BindingExpressions.Add(new ActionExpression<TElement>(function));
        
        return this;
    }

    public ICompiledDocumentSchema<TElement, TSelector, TModel> Build()
    {
        return new CompiledDocumentSchema<TElement, TSelector, TModel>(
            new BindObjectExpression<TElement, TSelector>(
                typeof(TModel),
                null,
                null,
                BindingExpressions.ToArray()));
    }
}