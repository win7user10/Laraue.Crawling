using System.Linq.Expressions;
using Laraue.Crawling.Abstractions;
using Laraue.Crawling.Abstractions.Schema;
using Laraue.Crawling.Common;
using Laraue.Crawling.Static.Abstractions;

namespace Laraue.Crawling.Static.Impl;

/// <summary>
/// Builder for the static html schema. "Static" means static html,
/// it will not use a browser for the crawling.
/// </summary>
/// <typeparam name="TElement"></typeparam>
/// <typeparam name="TModel"></typeparam>
public class StaticHtmlSchemaBuilder<TElement, TModel>
{
    private readonly List<BindExpression<TElement>> _bindingExpressions = new ();
    
    public StaticHtmlSchemaBuilder<TElement, TModel> HasProperty<TValue>(
        Expression<Func<TModel, TValue>> schemaProperty,
        HtmlSelector htmlSelector,
        GetValueDelegate<TElement, TValue> mapFunction)
    {
        var property = Helper.GetParsingProperty(schemaProperty);
        
        var bindingExpression = new BindValueExpression<TElement>(
            typeof(TValue),
            (target, value) => property.SetValue(target, value, null),
            htmlSelector,
            async element => await mapFunction.Invoke(element));
        
        _bindingExpressions.Add(bindingExpression);

        return this;
    }

    public StaticHtmlSchemaBuilder<TElement, TModel> HasProperty<TValue>(
        Expression<Func<TModel, TValue>> schemaProperty,
        HtmlSelector htmlSelector,
        Action<StaticHtmlSchemaBuilder<TElement, TValue>> childBuilder)
    {
        var property = Helper.GetParsingProperty(schemaProperty);
        
        var internalSchema = GetInternalSchema(
            childBuilder,
            (target, value) => property.SetValue(target, value, null));
        
        var bindingExpression = new BindObjectExpression<TElement>(
            typeof(TValue),
            (target, value) => property.SetValue(target, value, null),
            htmlSelector,
            internalSchema.ChildPropertiesBinders);
        
        _bindingExpressions.Add(bindingExpression);

        return this;
    }

    public StaticHtmlSchemaBuilder<TElement, TModel> HasArrayProperty<TValue>(
        Expression<Func<TModel, TValue[]>> schemaProperty,
        HtmlSelector htmlSelector,
        GetValueDelegate<TElement?, TValue> mapFunction)
    {
        var property = Helper.GetParsingProperty(schemaProperty);
        
        var bindingExpression = new BindArrayExpression<TElement>(
            typeof(TValue),
            (target, value) => property.SetValue(target, value, null),
            htmlSelector,
            new BindValueExpression<TElement>(
                typeof(TValue),
                (target, value) => property.SetValue(target, value, null),
                null,
                async element => await mapFunction.Invoke(element)));
        
        _bindingExpressions.Add(bindingExpression);

        return this;
    }

    public StaticHtmlSchemaBuilder<TElement, TModel> HasArrayProperty<TValue>(
        Expression<Func<TModel, TValue[]>> schemaProperty,
        HtmlSelector htmlSelector,
        Action<StaticHtmlSchemaBuilder<TElement, TValue>> childBuilder)
    {
        var property = Helper.GetParsingProperty(schemaProperty);

        var internalSchema = GetInternalSchema(childBuilder);
        
        var bindingExpression = new BindArrayExpression<TElement>(
            typeof(TValue),
            (target, value) => property.SetValue(target, value, null),
            htmlSelector,
            internalSchema);
        
        _bindingExpressions.Add(bindingExpression);

        return this;
    }
    
    private BindObjectExpression<TElement> GetInternalSchema<TValue>(
        Action<StaticHtmlSchemaBuilder<TElement, TValue>> childBuilder,
        SetPropertyDelegate? propertySetter = null)
    {
        var internalSchemaBuilder = new StaticHtmlSchemaBuilder<TElement, TValue>();
        
        childBuilder(internalSchemaBuilder);

        return new BindObjectExpression<TElement>(
            typeof(TValue),
            propertySetter,
            null,
            internalSchemaBuilder._bindingExpressions.ToArray());
    }

    public ICompiledStaticHtmlSchema<TElement, TModel> Build()
    {
        return new CompiledStaticHtmlSchema<TElement, TModel>(
            new BindObjectExpression<TElement>(
                typeof(TModel),
                null,
                null,
                _bindingExpressions.ToArray()));
    }
}