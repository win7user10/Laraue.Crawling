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
    where TModel : class, ICrawlingModel
{
    private readonly PropertyBuilderFactory<TElement> _propertyBuilderFactory;
    public readonly List<SchemaExpression<TElement>> BindingExpressions = new ();

    public DocumentSchemaBuilder(PropertyBuilderFactory<TElement> propertyBuilderFactory)
    {
        _propertyBuilderFactory = propertyBuilderFactory;
    }

    public DocumentSchemaBuilder<TElement, TSelector, TModel> HasProperty<TValue>(
        Expression<Func<TModel, TValue?>> schemaProperty,
        TSelector selector)
    {
        return HasProperty(schemaProperty, builder => builder.UseSelector(selector));
    }
    
    public DocumentSchemaBuilder<TElement, TSelector, TModel> HasProperty<TValue>(
        Expression<Func<TModel, TValue?>> schemaProperty,
        Action<PropertyBuilder<TElement, TSelector, TModel, TValue>> setupPropertyBuilder)
    {
        var propertyBuilder = _propertyBuilderFactory.GetPropertyBuilder<TSelector, TModel, TValue>();
        
        setupPropertyBuilder(propertyBuilder);
        
        var property = Helper.GetParsingProperty(schemaProperty);

        SchemaExpression<TElement> bindingExpression;
        if (Helper.TryGetArrayDefinition(typeof(TValue), out var arrayType))
        {
            bindingExpression = new BindArrayExpression<TElement, TSelector>(
                arrayType,
                new SetPropertyInfo(property),
                propertyBuilder.Selector,
                new BindValueExpression<TElement, TSelector>(
                    arrayType,
                    new SetPropertyInfo(property),
                    selector: null,
                    async element => await propertyBuilder.Extractors.GetValueAsync(element, arrayType)));
        }
        else
        {
            bindingExpression = new BindValueExpression<TElement, TSelector>(
                typeof(TValue),
                new SetPropertyInfo(property),
                propertyBuilder.Selector,
                async element => await propertyBuilder.GetValueAsyncDelegate.Invoke(element).ConfigureAwait(false));
        }
        
        BindingExpressions.Add(bindingExpression);

        return this;
    }

    public DocumentSchemaBuilder<TElement, TSelector, TModel> HasObjectProperty<TValue>(
        Expression<Func<TModel, TValue?>> schemaProperty,
        TSelector? htmlSelector,
        Action<DocumentSchemaBuilder<TElement, TSelector, TValue>> childBuilder)
        where TValue : class, ICrawlingModel
    {
        var property = Helper.GetParsingProperty(schemaProperty);
        
        var internalSchema = GetInternalSchema<TValue>(
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
        GetValueAsyncDelegate<TElement?, TValue> mapFunction)
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
        where TValue : class, ICrawlingModel
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
        where TValue : class, ICrawlingModel
    {
        var internalSchemaBuilder = new DocumentSchemaBuilder<TElement, TSelector, TValue>(_propertyBuilderFactory);
        
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