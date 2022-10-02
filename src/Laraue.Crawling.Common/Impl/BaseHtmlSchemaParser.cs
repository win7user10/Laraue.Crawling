using Laraue.Crawling.Abstractions;
using Laraue.Crawling.Abstractions.Schema;
using Laraue.Crawling.Abstractions.Schema.Binding;

namespace Laraue.Crawling.Common.Impl;

public abstract class BaseHtmlSchemaParser<TElement>
{
    /// <summary>
    /// Parse passed schema and return the result.
    /// </summary>
    /// <param name="schema"></param>
    /// <param name="rootElement"></param>
    /// <typeparam name="TModel"></typeparam>
    /// <returns></returns>
    public async Task<TModel?> RunAsync<TModel>(
        ICompiledHtmlSchema<TElement, TModel> schema,
        TElement? rootElement)
    {
        return (TModel?) await ParseAsync((BindExpression<TElement>)schema.BindingExpression, rootElement)
            .ConfigureAwait(false);
    }
    
    /// <summary>
    /// Method for routing between all possible parsing expressions.
    /// </summary>
    /// <param name="bindingExpression"></param>
    /// <param name="document"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    private Task<object?> ParseAsync(BindExpression<TElement> bindingExpression, TElement? document)
    {
        if (document is null)
        {
            return Task.FromResult((object?) null);
        }
        
        return bindingExpression switch
        {
            BindObjectExpression<TElement> complexType => ParseAsync(complexType, document),
            BindArrayExpression<TElement> arrayType => ParseAsync(arrayType, document),
            BindValueExpression<TElement> simpleType => ParseAsync(simpleType, document),
            _ => throw new NotImplementedException()
        };
    }

    /// <summary>
    /// Returns element retrieved from the current element by passed selector.  
    /// </summary>
    /// <param name="currentElement"></param>
    /// <param name="htmlSelector"></param>
    /// <returns></returns>
    protected abstract Task<TElement?> GetElementAsync(TElement currentElement, HtmlSelector htmlSelector);
    
    /// <summary>
    /// Returns elements retrieved from the current element by passed selector. 
    /// </summary>
    /// <param name="currentElement"></param>
    /// <param name="htmlSelector"></param>
    /// <returns></returns>
    protected abstract Task<TElement[]?> GetElementsAsync(TElement currentElement, HtmlSelector htmlSelector);
    
    private async Task<object?> ParseAsync(BindObjectExpression<TElement> complexType, TElement document)
    {
        var intermediateNode = complexType.HtmlSelector is not null
            ? await GetElementAsync(document,complexType.HtmlSelector.Selector).ConfigureAwait(false)
            : document;

        if (intermediateNode is null)
        {
            return null;
        }

        var objectInstance = Helper.GetInstanceOfType(complexType.ObjectType);
        var objectBinder = ObjectBinderFactory.ForObject(objectInstance);
        
        foreach (var element in complexType.ChildPropertiesBinders)
        {
            await ProcessSchemaExpressionAsync(element, document, objectBinder, objectInstance);
        }

        return objectInstance;
    }

    protected virtual Task ProcessSchemaExpressionAsync(
        SchemaExpression<TElement> schemaExpression,
        TElement currentElement,
        IObjectBinder objectBinder,
        object objectInstance)
    {
        return schemaExpression switch
        {
            BindExpression<TElement> bindExpression => ProcessBindExpressionAsync(
                bindExpression,
                currentElement,
                objectInstance),
            ActionExpression<TElement> actionExpression => ProcessActionExpressionAsync(
                actionExpression,
                currentElement),
            ManualBindExpression<TElement> manualBindExpression => ProcessManualBindExpressionAsync(
                manualBindExpression,
                currentElement,
                objectBinder),
            _ => throw new InvalidOperationException(),
        };
    }
    
    private async Task ProcessBindExpressionAsync(
        BindExpression<TElement> bindExpression,
        TElement currentElement,
        object objectInstance)
    {
        var value = await ParseAsync(bindExpression, currentElement).ConfigureAwait(false);
        bindExpression.PropertySetter?.Invoke(objectInstance, value);
    }
    
    private Task ProcessActionExpressionAsync(
        ActionExpression<TElement> actionExpression,
        TElement currentElement)
    {
        return actionExpression.AsyncAction(currentElement);
    }
    
    private Task ProcessManualBindExpressionAsync(
        ManualBindExpression<TElement> actionExpression,
        TElement currentElement,
        IObjectBinder objectBinder)
    {
        return (Task)actionExpression.AsyncBindFunction.DynamicInvoke(currentElement, objectBinder)!;
    }

    private async Task<object?> ParseAsync(BindValueExpression<TElement> simpleType, TElement document)
    {
        var documentToParse = document;
        
        if (simpleType.HtmlSelector is not null)
        {
            documentToParse = await GetElementAsync(document, simpleType.HtmlSelector.Selector).ConfigureAwait(false);
        }

        if (documentToParse is null)
        {
            return null;
        }

        return await simpleType.PropertyGetter(documentToParse);
    }
    
    private async Task<object?> ParseAsync(BindArrayExpression<TElement> arrayType, TElement document)
    {
        TElement[]? children = null;
        if (arrayType.HtmlSelector is not null)
        {
            children = await GetElementsAsync(document, arrayType.HtmlSelector.Selector).ConfigureAwait(false);
        }
        
        if (children is null)
        {
            return null;
        }

        var result = (object?[])Array.CreateInstance(arrayType.ObjectType, children.Length);

        for (var i = 0; i < children.Length; i++)
        {
            var child = children[i];
            var value = await ParseAsync(arrayType.Element, child).ConfigureAwait(false);
            result[i] = value;
        }
        
        return result;
    }
}