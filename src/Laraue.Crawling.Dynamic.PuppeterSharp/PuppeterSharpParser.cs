using Laraue.Crawling.Abstractions;
using Laraue.Crawling.Common;
using PuppeteerSharp;

namespace Laraue.Crawling.Dynamic.PuppeterSharp;

public class PuppeterSharpParser
{
    public async Task<TModel> VisitAsync<TModel, TPage>(TPage page, ICompiledDynamicHtmlSchema<TModel, TPage, ElementHandle> schema) where TPage : Page
    {
        var element = await page.QuerySelectorAsync("body");

        return (TModel) await ParseAsync(page, schema, element).ConfigureAwait(false);
    }
    
    private Task<object?> ParseAsync<TPage>(TPage page, ParseExpression<ElementHandle> parseExpression, ElementHandle element)
    {
        return parseExpression switch
        {
            ArrayParseExpression<ElementHandle> arrayType => ParseAsync(page, arrayType, element),
            SimpleTypeParseExpression<ElementHandle> simpleType => ParseAsync(page, simpleType, element),
            ComplexTypeParseExpression<ElementHandle> complexType => ParseAsync(page, complexType, element),
            _ => throw new NotImplementedException()
        };
    }
    
    private async Task<object?> ParseAsync<TPage>(TPage page, SimpleTypeParseExpression<ElementHandle> parseExpression, ElementHandle element)
    {
        if (parseExpression.HtmlSelector is not null)
        {
            element = await element.QuerySelectorAsync(parseExpression.HtmlSelector.Selector).ConfigureAwait(false);
        }
        
        return await parseExpression.AsyncPropertyGetter(element);
    }

    private async Task<object?> ParseAsync<TPage>(TPage page, ArrayParseExpression<ElementHandle> parseExpression, ElementHandle element)
    {
        ElementHandle[]? children = null;
        if (parseExpression.HtmlSelector is not null)
        {
            children = await element.QuerySelectorAllAsync(parseExpression.HtmlSelector.Selector).ConfigureAwait(false);
        }
        
        if (children is null)
        {
            return null;
        }

        var result = (object?[])Array.CreateInstance(parseExpression.ObjectType, children.Length);

        for (var i = 0; i < children.Length; i++)
        {
            var child = children[i];
            var value = await ParseAsync(page, parseExpression.Element, child).ConfigureAwait(false);
            result[i] = value;
        }
        
        return result;
    }
    
    private async Task<object?> ParseAsync<TPage>(TPage page, ComplexTypeParseExpression<ElementHandle> parseExpression, ElementHandle element)
    {
        var intermediateNode = parseExpression.HtmlSelector is not null
            ? await element.QuerySelectorAsync(parseExpression.HtmlSelector.Selector)
            : element;

        if (intermediateNode is null)
        {
            return null;
        }

        return await ParseAsync(page, (IObjectElement) parseExpression, intermediateNode).ConfigureAwait(false);
    }

    private async Task<object?> ParseAsync<TPage>(TPage page, IObjectElement objectElement, ElementHandle element)
    {
        var objectInstance = Helper.GetInstanceOfType(objectElement.ObjectType);

        var objectBinderType = typeof(ObjectBinder<>);
        var typeArgs = new []{ objectElement.ObjectType };
        var genericObjectBinderType = objectBinderType.MakeGenericType(typeArgs);
        var objectBinder = Activator.CreateInstance(genericObjectBinderType, objectInstance);

        foreach (var action in objectElement.Actions)
        {
            switch (action)
            {
                case PageAction<TPage> pageAction:
                    await pageAction.AsyncAction(page);
                    break;
                case ComplexTypeBindAction bindAction:
                    await ((Task)bindAction.AsyncBindFunction.DynamicInvoke(page, element, objectBinder)!).ConfigureAwait(false);
                    break;
                case ParseExpression<ElementHandle> childParseExpression:
                {
                    var value = await ParseAsync(page, childParseExpression, element).ConfigureAwait(false);
                    childParseExpression.PropertySetter(objectInstance, value);
                    break;
                }
            }
        }

        return objectInstance;
    }
}