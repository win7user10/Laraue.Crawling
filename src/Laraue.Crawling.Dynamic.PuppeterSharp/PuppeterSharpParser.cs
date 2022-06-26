using PuppeteerSharp;

namespace Laraue.Crawling.Dynamic.PuppeterSharp;

public class PuppeterSharpParser
{
    public async Task<TModel> VisitAsync<TModel, TPage>(TPage page, ICompiledDynamicHtmlSchema<TModel, TPage> builder) where TPage : Page
    {
        var element = await page.QuerySelectorAsync("body");
        
        foreach (var action in builder.Actions)
        {
            if (action is PageAction<TPage> pageAction)
            {
                await pageAction.AsyncAction(page);
            }

            else if (action is ParseExpression parseExpression)
            {
                return (TModel) await ParseAsync(parseExpression, element);
            }
        }
        
        throw new Exception();
    }
    
    private Task<object?> ParseAsync(ParseExpression parseExpression, ElementHandle element)
    {
        return parseExpression switch
        {
            ArrayParseExpression arrayType => ParseAsync(arrayType, element),
            SimpleTypeParseExpression simpleType => ParseAsync(simpleType, element),
            _ => throw new NotImplementedException()
        };
    }

    private async Task<object?> ParseAsync(ArrayParseExpression parseExpression, ElementHandle element)
    {
        ElementHandle[]? children = null;
        if (parseExpression.HtmlSelector is not null)
        {
            children = await element.QuerySelectorAllAsync(parseExpression.HtmlSelector.Selector);
        }
        
        if (children is null)
        {
            return null;
        }

        var result = (object?[])Array.CreateInstance(parseExpression.ObjectType, children.Length);

        for (var i = 0; i < children.Length; i++)
        {
            var child = children[i];
            var value = await ParseAsync(parseExpression.Element, child);
            result[i] = value;
        }
        
        return result;
    }
}