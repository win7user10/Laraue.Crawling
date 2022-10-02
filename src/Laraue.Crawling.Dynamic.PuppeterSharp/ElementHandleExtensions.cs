using Laraue.Crawling.Common.Extensions;
using PuppeteerSharp;

namespace Laraue.Crawling.Dynamic.PuppeterSharp;

public static class ElementHandleExtensions
{
    public static Task<string> GetInnerTextAsync(this ElementHandle elementHandle)
    {
        return elementHandle.EvaluateFunctionAsync<string>("e => e.innerText");
    }
    
    public static Task<string[]> GetClassesAsync(this ElementHandle elementHandle)
    {
        return elementHandle.EvaluateFunctionAsync<string>("e => e.className").AwaitAndModify(x => x.Split(" "));
    }
    
    public static async Task<bool> HasClassAsync(this ElementHandle elementHandle, string className)
    {
        var classes = await elementHandle.GetClassesAsync();

        return classes.Contains(className);
    }
    
    public static Task<string> GetAttributeValueAsync(this ElementHandle elementHandle, string attributeName)
    {
        return elementHandle.EvaluateFunctionAsync<string>($"e => e.attributes.{attributeName}.nodeValue");
    }
    
    public static async Task<ElementHandle?> QuerySelectorByIndexAsync(this ElementHandle elementHandle, string selector, int index)
    {
        var handle = await elementHandle.EvaluateFunctionHandleAsync($"e => e.querySelectorAll('{selector}')[{index}]").ConfigureAwait(false);
        
        if (handle is ElementHandle element)
        {
            return element;
        }

        await handle.DisposeAsync().ConfigureAwait(false);
        return null;
    }
    
    public static Task<int> GetElementsCountAsync(this ElementHandle elementHandle, string selector)
    {
        return elementHandle.EvaluateFunctionAsync<int>($"e => e.querySelectorAll('{selector}').length");
    }
    
    public static Task<string> GetTrimmedInnerTextAsync(this ElementHandle elementHandle)
    {
        return GetInnerTextAsync(elementHandle).AwaitAndModify(s => s.Trim());
    }
    
    public static Task<int> GetIntAsync(this ElementHandle elementHandle)
    {
        return GetInnerTextAsync(elementHandle).AwaitAndModify(RetrieveExtensions.GetIntOrDefault);
    }
    
    public static Task<decimal> GetDecimalAsync(this ElementHandle elementHandle)
    {
        return GetInnerTextAsync(elementHandle).AwaitAndModify(RetrieveExtensions.GetDecimalOrDefault);
    }
}