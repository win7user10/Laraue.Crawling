using System.Diagnostics;
using Laraue.Crawling.Abstractions;
using Laraue.Crawling.Abstractions.Schema;
using Laraue.Crawling.Abstractions.Schema.Binding;
using Microsoft.Extensions.Logging;

namespace Laraue.Crawling.Common.Impl;

/// <summary>
/// Base functionality to implement <see cref="IDocumentSchemaParser{TElement,TSelector}"/>.
/// </summary>
public abstract class BaseDocumentSchemaParser<TElement, TSelector>
    : IDocumentSchemaParser<TElement, TSelector>
    where TSelector : Selector
{
    private readonly ILogger<BaseDocumentSchemaParser<TElement, TSelector>> _logger;
    private readonly ILoggerFactory _loggerFactory;

    /// <summary>
    /// Initializes a new instance of <see cref="BaseDocumentSchemaParser{TElement,TSelector}"/>.
    /// </summary>
    /// <param name="loggerFactory"></param>
    protected BaseDocumentSchemaParser(ILoggerFactory loggerFactory)
    {
        _loggerFactory = loggerFactory;
        _logger = loggerFactory.CreateLogger<BaseDocumentSchemaParser<TElement, TSelector>>();
    }

    /// <inheritdoc />
    public async Task<TModel?> RunAsync<TModel>(
        ICompiledDocumentSchema<TElement, TSelector, TModel> schema,
        TElement? rootElement)
    {
        var stopWatch = new Stopwatch();
        stopWatch.Start();
        
        _logger.LogTrace("Bind started");
        
        var result = (TModel?) await ParseAsync(
                (BindExpression<TElement, TSelector>)schema.BindingExpression,
                rootElement,
                new VisitorContext())
            .ConfigureAwait(false);
        
        stopWatch.Stop();
        
        _logger.LogDebug("Bind finished for {Time}", stopWatch.Elapsed);

        return result;
    }

    /// <inheritdoc />
    public async Task<TModel?> RunAsync<TModel>(ICompiledElementSchema<TElement, TSelector, TModel> schema, TElement? rootElement)
    {
        var result = await RunAsync(schema.ObjectSchema, rootElement).ConfigureAwait(false);

        return result.Value;
    }

    /// <summary>
    /// Method for routing between all possible parsing expressions.
    /// </summary>
    /// <param name="bindingExpression"></param>
    /// <param name="document"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    private Task<object?> ParseAsync(
        BindExpression<TElement, TSelector> bindingExpression,
        TElement? document,
        VisitorContext context)
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

        try
        {
            return bindingExpression switch
            {
                BindObjectExpression<TElement, TSelector> complexType => ParseAsync(complexType, document, childContext),
                BindArrayExpression<TElement, TSelector> arrayType => ParseAsync(arrayType, document, childContext),
                BindValueExpression<TElement, TSelector> simpleType => ParseAsync(simpleType, document, childContext),
                _ => throw new InvalidOperationException()
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Value {Path} binding error", context.ToString());

            throw;
        }
    }

    /// <summary>
    /// Returns element retrieved from the current element by passed selector.  
    /// </summary>
    /// <param name="currentElement"></param>
    /// <param name="selector"></param>
    /// <returns></returns>
    protected abstract Task<TElement?> GetElementAsync(TElement currentElement, TSelector selector);

    private async Task<TElement?> GetElementInternalAsync(TElement currentElement, TSelector selector, VisitorContext context)
    {
        var result = await GetElementAsync(currentElement, selector).ConfigureAwait(false);
        
        _logger.LogTrace("Select Property: {Path} Selector: {Selector} Value: {Value}",
            context,
            selector,
            result);

        return result;
    }
    
    /// <summary>
    /// Returns elements retrieved from the current element by passed selector. 
    /// </summary>
    /// <param name="currentElement"></param>
    /// <param name="selector"></param>
    /// <returns></returns>
    protected abstract Task<TElement[]?> GetElementsAsync(TElement currentElement, TSelector selector);
    
    private async Task<TElement[]?> GetElementsInternalAsync(TElement currentElement, TSelector selector, VisitorContext context)
    {
        var result = await GetElementsAsync(currentElement, selector).ConfigureAwait(false);
        
        _logger.LogTrace("Select multiple Property: {Path} Selector: {Selector} Count: {Count}",
            context,
            selector,
            result?.Length);

        return result;
    }
    
    private async Task<object?> ParseAsync(BindObjectExpression<TElement, TSelector> complexType, TElement document, VisitorContext context)
    {
        var intermediateNode = complexType.Selector is not null
            ? await GetElementInternalAsync(document, complexType.Selector, context).ConfigureAwait(false)
            : document;

        if (intermediateNode is null)
        {
            return null;
        }

        var objectInstance = Helper.GetInstanceOfType(complexType.ObjectType);
        var objectBinder = ObjectBinderFactory.ForObject(objectInstance, _loggerFactory, context);
        
        foreach (var element in complexType.ChildPropertiesBinders)
        {
            await ProcessSchemaExpressionAsync(element, intermediateNode, objectBinder, objectInstance, context)
                .ConfigureAwait(false);
        }

        return objectInstance;
    }

    private Task ProcessSchemaExpressionAsync(
        SchemaExpression<TElement> schemaExpression,
        TElement currentElement,
        IObjectBinder objectBinder,
        object objectInstance,
        VisitorContext visitorContext)
    {
        return schemaExpression switch
        {
            BindExpression<TElement, TSelector> bindExpression => ProcessBindExpressionAsync(
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
        BindExpression<TElement, TSelector> bindExpression,
        TElement currentElement,
        object objectInstance,
        VisitorContext visitorContext)
    {
        var value = await ParseAsync(bindExpression, currentElement, visitorContext).ConfigureAwait(false);

        var currentContext = visitorContext.Push(bindExpression.SetPropertyInfo!.PropertyInfo.Name);
        
        _logger.LogTrace("Bind Property: {Path} Value: {Value}", currentContext, value);
        
        bindExpression.SetPropertyInfo?.PropertyInfo.SetValue(objectInstance, value);
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
        BindValueExpression<TElement, TSelector> simpleType,
        TElement document,
        VisitorContext context)
    {
        var documentToParse = document;
        
        if (simpleType.Selector is not null)
        {
            documentToParse = await GetElementInternalAsync(document, simpleType.Selector, context)
                .ConfigureAwait(false);
        }

        if (documentToParse is null)
        {
            return null;
        }

        try
        {
            return await simpleType.PropertyGetter(documentToParse).ConfigureAwait(false);
        }
        catch (Exception e)
        {
            throw new InvalidOperationException($"The property '{context}' could not be parsed: {e.Message}", e);
        }
    }
    
    private static async Task<object?> ParseAsync(
        ReturnExpression<TElement, TSelector> returnExpression,
        TElement? document)
    {
        if (document is null)
        {
            return null;
        }
        
        return await returnExpression.ReturnFunction.Invoke(document);
    }
    
    private async Task<object?> ParseAsync(
        BindArrayExpression<TElement, TSelector> arrayType,
        TElement document,
        VisitorContext context)
    {
        TElement[]? children = null;
        if (arrayType.Selector is not null)
        {
            children = await GetElementsInternalAsync(document, arrayType.Selector, context)
                .ConfigureAwait(false);
        }
        
        if (children is null)
        {
            return null;
        }

        var result = Array.CreateInstance(arrayType.ObjectType, children.Length);

        for (var i = 0; i < children.Length; i++)
        {
            var childContext = context.Push(i);
            
            var child = children[i];
            var value = await ParseAsync(arrayType.Element, child, childContext).ConfigureAwait(false);
            result.SetValue(value, i);
        }
        
        return result;
    }
}