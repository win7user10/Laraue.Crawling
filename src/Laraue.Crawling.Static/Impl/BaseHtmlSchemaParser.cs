using Laraue.Crawling.Abstractions;
using Laraue.Crawling.Abstractions.Schema;
using Laraue.Crawling.Common;
using Laraue.Crawling.Static.Abstractions;

namespace Laraue.Crawling.Static.Impl;

public abstract class BaseHtmlSchemaParser<TElement>
{
    /// <summary>
    /// Parse passed schema and return the result.
    /// </summary>
    /// <param name="schema"></param>
    /// <param name="rootElement"></param>
    /// <typeparam name="TModel"></typeparam>
    /// <returns></returns>
    public abstract Task<TModel?> ParseAsync<TModel>(
        ICompiledStaticHtmlSchema<TElement, TModel> schema,
        TElement? rootElement);
    
    /// <summary>
    /// Method for routing between all possible parsing expressions.
    /// </summary>
    /// <param name="bindingExpression"></param>
    /// <param name="document"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    protected Task<object?> ParseAsync(BindExpression<TElement> bindingExpression, TElement? document)
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
            ? await GetElementAsync(document,complexType.HtmlSelector.Selector)
            : document;

        if (intermediateNode is null)
        {
            return null;
        }

        var objectInstance = Helper.GetInstanceOfType(complexType.ObjectType);
        foreach (var element in complexType.ChildPropertiesBinders)
        {
            var value = await ParseAsync(element, intermediateNode);
            element.PropertySetter?.Invoke(objectInstance, value);
        }

        return objectInstance;
    }
    
    private async Task<object?> ParseAsync(BindValueExpression<TElement> simpleType, TElement document)
    {
        var documentToParse = document;
        
        if (simpleType.HtmlSelector is not null)
        {
            documentToParse = await GetElementAsync(document, simpleType.HtmlSelector.Selector);
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
            children = await GetElementsAsync(document, arrayType.HtmlSelector.Selector);
        }
        
        if (children is null)
        {
            return null;
        }

        var result = (object?[])Array.CreateInstance(arrayType.ObjectType, children.Length);

        for (var i = 0; i < children.Length; i++)
        {
            var child = children[i];
            var value = await ParseAsync(arrayType.Element, child);
            result[i] = value;
        }
        
        return result;
    }
}