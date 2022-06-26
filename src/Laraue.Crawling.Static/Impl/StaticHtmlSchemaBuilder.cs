using System.Linq.Expressions;
using System.Reflection;
using Laraue.Crawling.Abstractions;
using Laraue.Crawling.Static.Abstractions;

namespace Laraue.Crawling.Static.Impl;

public class StaticHtmlSchemaBuilder<TModel> : IStaticHtmlSchemaBuilder<TModel>
{
    private readonly List<BindingExpression> _bindingExpressions = new ();

    public IStaticHtmlSchemaBuilder<TModel> HasProperty<TValue>(
        Expression<Func<TModel, TValue>> schemaProperty,
        HtmlSelector htmlSelector,
        Func<IHtmlElement?, TValue?> mapFunction)
    {
        var property = Helper.GetParsingProperty(schemaProperty);
        
        var bindingExpression = new SimpleTypeBindingExpression(
            (target, value) => property.SetValue(target, value, null),
            value => mapFunction(value),
            htmlSelector,
            typeof(TValue));
        
        _bindingExpressions.Add(bindingExpression);

        return this;
    }

    public IStaticHtmlSchemaBuilder<TModel> HasProperty<TValue>(Expression<Func<TModel, TValue>> schemaProperty, HtmlSelector htmlSelector, Action<IStaticHtmlSchemaBuilder<TValue>> childBuilder)
    {
        var property = Helper.GetParsingProperty(schemaProperty);
        var internalSchema = GetInternalSchema(childBuilder, (target, value) 
            => property.SetValue(target, value, null));
        
        var bindingExpression = new ComplexTypeBindingExpression(
            (target, value) => property.SetValue(target, value, null),
            htmlSelector,
            typeof(TValue),
            internalSchema.Elements);
        
        _bindingExpressions.Add(bindingExpression);

        return this;
    }

    public IStaticHtmlSchemaBuilder<TModel> HasArrayProperty<TValue>(Expression<Func<TModel, TValue[]>> schemaProperty, HtmlSelector htmlSelector, Func<IHtmlElement?, TValue?> mapFunction)
    {
        var property = Helper.GetParsingProperty(schemaProperty);
        
        var bindingExpression = new ArrayBindingExpression(
            (target, value) => property.SetValue(target, value, null),
            htmlSelector,
            typeof(TValue),
            new SimpleTypeBindingExpression(
                (target, value) => property.SetValue(target, value, null),
                value => mapFunction(value),
                null,
                typeof(TValue)));
        
        _bindingExpressions.Add(bindingExpression);

        return this;
    }

    public IStaticHtmlSchemaBuilder<TModel> HasArrayProperty<TValue>(Expression<Func<TModel, TValue[]>> schemaProperty, HtmlSelector htmlSelector, Action<IStaticHtmlSchemaBuilder<TValue>> childBuilder)
    {
        var property = Helper.GetParsingProperty(schemaProperty);

        var internalSchema = GetInternalSchema(childBuilder, (target, value) =>
        {
            
        });
        
        var bindingExpression = new ArrayBindingExpression(
            (target, value) => property.SetValue(target, value, null),
            htmlSelector,
            typeof(TValue),
            internalSchema);
        
        _bindingExpressions.Add(bindingExpression);

        return this;
    }
    
    private ComplexTypeBindingExpression GetInternalSchema<TValue>(Action<IStaticHtmlSchemaBuilder<TValue>> childBuilder, Action<object, object?> propertySetter)
    {
        var internalSchemaBuilder = new StaticHtmlSchemaBuilder<TValue>();
        
        childBuilder(internalSchemaBuilder);

        return new ComplexTypeBindingExpression(
            propertySetter,
            null,
            typeof(TValue),
            internalSchemaBuilder._bindingExpressions.ToArray());
    }

    public ICompiledStaticHtmlSchema<TModel> Build()
    {
        return new CompiledStaticHtmlSchema<TModel>(
            new ComplexTypeBindingExpression(
                null,
                null,
                typeof(TModel),
                _bindingExpressions.ToArray()));
    }
}