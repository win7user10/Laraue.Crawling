using System.Linq.Expressions;
using System.Reflection;
using Laraue.Crawling.Static.Abstractions;

namespace Laraue.Crawling.Static.Impl;

public class HtmlSchemaBuilder<TModel> : IHtmlSchemaBuilder<TModel>
{
    private readonly List<BindingExpression> _bindingExpressions = new ();

    public IHtmlSchemaBuilder<TModel> HasProperty<TValue>(
        Expression<Func<TModel, TValue>> schemaProperty,
        HtmlSelector htmlSelector,
        Func<IHtmlElement?, TValue?> mapFunction)
    {
        var property = GetBindingProperty(schemaProperty);
        
        var bindingExpression = new SimpleTypeBindingExpression(
            (target, value) => property.SetValue(target, value, null),
            value => mapFunction(value),
            htmlSelector,
            typeof(TValue));
        
        _bindingExpressions.Add(bindingExpression);

        return this;
    }

    public IHtmlSchemaBuilder<TModel> HasProperty<TValue>(Expression<Func<TModel, TValue>> schemaProperty, HtmlSelector htmlSelector, Action<IHtmlSchemaBuilder<TValue>> childBuilder)
    {
        var property = GetBindingProperty(schemaProperty);
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

    public IHtmlSchemaBuilder<TModel> HasArrayProperty<TValue>(Expression<Func<TModel, TValue[]>> schemaProperty, HtmlSelector htmlSelector, Func<IHtmlElement?, TValue?> mapFunction)
    {
        var property = GetBindingProperty(schemaProperty);
        
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

    public IHtmlSchemaBuilder<TModel> HasArrayProperty<TValue>(Expression<Func<TModel, TValue[]>> schemaProperty, HtmlSelector htmlSelector, Action<IHtmlSchemaBuilder<TValue>> childBuilder)
    {
        var property = GetBindingProperty(schemaProperty);

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
    
    private ComplexTypeBindingExpression GetInternalSchema<TValue>(Action<IHtmlSchemaBuilder<TValue>> childBuilder, Action<object, object?> propertySetter)
    {
        var internalSchemaBuilder = new HtmlSchemaBuilder<TValue>();
        
        childBuilder(internalSchemaBuilder);

        return new ComplexTypeBindingExpression(
            propertySetter,
            null,
            typeof(TValue),
            internalSchemaBuilder._bindingExpressions.ToArray());
    }

    private PropertyInfo GetBindingProperty<TValue>(Expression<Func<TModel, TValue>> schemaProperty)
    {
        if (schemaProperty.Body is not MemberExpression memberSelectorExpression)
        {
            throw new NotImplementedException();
        }
        
        var property = memberSelectorExpression.Member as PropertyInfo;
        return property ?? throw new NotImplementedException();
    }

    public ICompiledHtmlSchema<TModel> Build()
    {
        return new CompiledHtmlSchema<TModel>(
            new ComplexTypeBindingExpression(
                null,
                null,
                typeof(TModel),
                _bindingExpressions.ToArray()));
    }
}