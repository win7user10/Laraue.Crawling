using System.Diagnostics;
using Laraue.Crawling.Abstractions;
using Laraue.Crawling.Abstractions.Schema;
using Laraue.Crawling.Abstractions.Schema.Binding;
using Microsoft.Extensions.Logging;

namespace Laraue.Crawling.Common.Impl;

public abstract class BaseHtmlSchemaParser<TElement>
{
    private readonly ILogger<BaseHtmlSchemaParser<TElement>> _logger;
    private readonly ILoggerFactory _loggerFactory;

    protected BaseHtmlSchemaParser(ILoggerFactory loggerFactory)
    {
        _loggerFactory = loggerFactory;
        _logger = loggerFactory.CreateLogger<BaseHtmlSchemaParser<TElement>>();
    }

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
        var stopWatch = new Stopwatch();
        stopWatch.Start();
        
        _logger.LogTrace("Bind started");
        
        var result = (TModel?) await ParseAsync((BindExpression<TElement>)schema.BindingExpression, rootElement, new VisitorContext())
            .ConfigureAwait(false);
        
        stopWatch.Stop();
        
        _logger.LogDebug("Binding finished for {Time}", stopWatch.Elapsed);

        return result;
    }
    
    /// <summary>
    /// Method for routing between all possible parsing expressions.
    /// </summary>
    /// <param name="bindingExpression"></param>
    /// <param name="document"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    private Task<object?> ParseAsync(BindExpression<TElement> bindingExpression, TElement? document, VisitorContext context)
    {
        if (document is null)
        {
            return Task.FromResult((object?) null);
        }

        var childContext = context;
        if (bindingExpression.SetPropertyInfo is not null)
        {
            childContext = childContext.Push(bindingExpression.SetPropertyInfo.PropertyInfo.Name);
        }
        
        return bindingExpression switch
        {
            BindObjectExpression<TElement> complexType => ParseAsync(complexType, document, childContext),
            BindArrayExpression<TElement> arrayType => ParseAsync(arrayType, document, childContext),
            BindValueExpression<TElement> simpleType => ParseAsync(simpleType, document, childContext),
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

    private async Task<TElement?> GetElementInternalAsync(TElement currentElement, HtmlSelector htmlSelector, VisitorContext context)
    {
        var result = await GetElementAsync(currentElement, htmlSelector).ConfigureAwait(false);
        
        _logger.LogTrace("Select Property: {Path} Selector: {Selector} Value: {Value}",
            context,
            htmlSelector,
            result);

        return result;
    }
    
    /// <summary>
    /// Returns elements retrieved from the current element by passed selector. 
    /// </summary>
    /// <param name="currentElement"></param>
    /// <param name="htmlSelector"></param>
    /// <returns></returns>
    protected abstract Task<TElement[]?> GetElementsAsync(TElement currentElement, HtmlSelector htmlSelector);
    
    private async Task<TElement[]?> GetElementsInternalAsync(TElement currentElement, HtmlSelector htmlSelector, VisitorContext context)
    {
        var result = await GetElementsAsync(currentElement, htmlSelector).ConfigureAwait(false);
        
        _logger.LogTrace("Select multiple Property: {Path} Selector: {Selector} Count: {Count}",
            context,
            htmlSelector,
            result?.Length);

        return result;
    }
    
    private async Task<object?> ParseAsync(BindObjectExpression<TElement> complexType, TElement document, VisitorContext context)
    {
        var intermediateNode = complexType.HtmlSelector is not null
            ? await GetElementInternalAsync(document,complexType.HtmlSelector.Selector, context).ConfigureAwait(false)
            : document;

        if (intermediateNode is null)
        {
            return null;
        }

        var objectInstance = Helper.GetInstanceOfType(complexType.ObjectType);
        var objectBinder = ObjectBinderFactory.ForObject(objectInstance, _loggerFactory, context);
        
        foreach (var element in complexType.ChildPropertiesBinders)
        {
            await ProcessSchemaExpressionAsync(element, document, objectBinder, objectInstance, context);
        }

        return objectInstance;
    }

    protected virtual Task ProcessSchemaExpressionAsync(
        SchemaExpression<TElement> schemaExpression,
        TElement currentElement,
        IObjectBinder objectBinder,
        object objectInstance,
        VisitorContext visitorContext)
    {
        return schemaExpression switch
        {
            BindExpression<TElement> bindExpression => ProcessBindExpressionAsync(
                bindExpression,
                currentElement,
                objectInstance,
                visitorContext),
            ActionExpression<TElement> actionExpression => ProcessActionExpressionAsync(
                actionExpression,
                currentElement,
                visitorContext),
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
        object objectInstance,
        VisitorContext visitorContext)
    {
        var value = await ParseAsync(bindExpression, currentElement, visitorContext).ConfigureAwait(false);

        var currentContext = visitorContext.Push(bindExpression.SetPropertyInfo!.PropertyInfo.Name);
        
        _logger.LogTrace("Bind Property: {Path} Value: {Value}", currentContext, value);
        
        bindExpression.SetPropertyInfo?.SetPropertyDelegate.Invoke(objectInstance, value);
    }
    
    private Task ProcessActionExpressionAsync(
        ActionExpression<TElement> actionExpression,
        TElement currentElement,
        VisitorContext visitorContext)
    {
        _logger.LogTrace("{Path} Execute action", visitorContext);
        
        return actionExpression.AsyncAction(currentElement);
    }
    
    private Task ProcessManualBindExpressionAsync(
        ManualBindExpression<TElement> actionExpression,
        TElement currentElement,
        IObjectBinder objectBinder)
    {
        return (Task)actionExpression.AsyncBindFunction.DynamicInvoke(currentElement, objectBinder)!;
    }

    private async Task<object?> ParseAsync(
        BindValueExpression<TElement> simpleType,
        TElement document,
        VisitorContext context)
    {
        var documentToParse = document;
        
        if (simpleType.HtmlSelector is not null)
        {
            documentToParse = await GetElementInternalAsync(document, simpleType.HtmlSelector.Selector, context)
                .ConfigureAwait(false);
        }

        if (documentToParse is null)
        {
            return null;
        }

        return await simpleType.PropertyGetter(documentToParse);
    }
    
    private async Task<object?> ParseAsync(
        BindArrayExpression<TElement> arrayType,
        TElement document,
        VisitorContext context)
    {
        TElement[]? children = null;
        if (arrayType.HtmlSelector is not null)
        {
            children = await GetElementsInternalAsync(document, arrayType.HtmlSelector.Selector, context)
                .ConfigureAwait(false);
        }
        
        if (children is null)
        {
            return null;
        }

        var result = (object?[])Array.CreateInstance(arrayType.ObjectType, children.Length);

        for (var i = 0; i < children.Length; i++)
        {
            var childContext = context.Push(i);
            
            var child = children[i];
            var value = await ParseAsync(arrayType.Element, child, childContext).ConfigureAwait(false);
            result[i] = value;
        }
        
        return result;
    }
}