using Laraue.Crawling.Common.Extensions;
using PuppeteerSharp;

namespace Laraue.Crawling.Dynamic.PuppeterSharp;

/// <summary>
/// Often used extensions to work with element handles.
/// </summary>
public static class ElementHandleExtensions
{
    /// <summary>
    /// Gets the inner text from the passed element handle.
    /// </summary>
    /// <param name="elementHandle"></param>
    /// <returns></returns>
    public static Task<string?> GetInnerTextAsync(this ElementHandle? elementHandle)
    {
        return elementHandle is null
            ? Task.FromResult<string?>(null)
            : elementHandle.EvaluateFunctionAsync<string?>("e => e.innerText");
    }
    
    /// <summary>
    /// Gets all class names from the passed element handle.
    /// </summary>
    /// <param name="elementHandle"></param>
    /// <returns></returns>
    public static Task<string[]> GetClassesAsync(this ElementHandle? elementHandle)
    {
        return elementHandle is null
            ? Task.FromResult(Array.Empty<string>())
            : elementHandle.EvaluateFunctionAsync<string>("e => e.className").AwaitAndModify(x => x.Split(" "));
    }
    
    /// <summary>
    /// Check does the passed element handle contains the passed <param name="className"></param>.
    /// </summary>
    /// <param name="elementHandle"></param>
    /// <param name="className"></param>
    /// <returns></returns>
    public static async Task<bool> HasClassAsync(this ElementHandle? elementHandle, string className)
    {
        var classes = await elementHandle.GetClassesAsync();

        return classes.Contains(className);
    }
    
    /// <summary>
    /// Get the attribute value if the attribute is exists.
    /// </summary>
    /// <param name="elementHandle"></param>
    /// <param name="attributeName"></param>
    /// <returns></returns>
    public static Task<string?> GetAttributeValueAsync(this ElementHandle? elementHandle, string attributeName)
    {
        return  elementHandle is null
            ? Task.FromResult<string?>(null)
            : elementHandle.EvaluateFunctionAsync<string?>($"e => e.attributes?.{attributeName}?.nodeValue");
    }
    
    /// <summary>
    /// Try to find elements by the passed selector and returns the item with the passed index from this list.
    /// </summary>
    /// <param name="elementHandle"></param>
    /// <param name="selector"></param>
    /// <param name="index"></param>
    /// <returns></returns>
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
    
    /// <summary>
    /// Get elements count of items for the passed selector.
    /// </summary>
    /// <param name="elementHandle"></param>
    /// <param name="selector"></param>
    /// <returns></returns>
    public static Task<int> GetElementsCountAsync(this ElementHandle? elementHandle, string selector)
    {
        return elementHandle is null
            ? Task.FromResult(0)
            : elementHandle.EvaluateFunctionAsync<int>($"e => e.querySelectorAll('{selector}').length");
    }
    
    /// <summary>
    /// Get inner text from the passed element and trim it.
    /// </summary>
    /// <param name="elementHandle"></param>
    /// <returns></returns>
    public static Task<string?> GetTrimmedInnerTextAsync(this ElementHandle? elementHandle)
    {
        return GetInnerTextAsync(elementHandle).AwaitAndModify(s => s?.Trim());
    }
    
    /// <summary>
    /// Get the inner text from the element and try to parse it as int or return the default int value.
    /// </summary>
    /// <param name="elementHandle"></param>
    /// <returns></returns>
    public static Task<int> GetIntAsync(this ElementHandle? elementHandle)
    {
        return GetInnerTextAsync(elementHandle).AwaitAndModify(RetrieveExtensions.GetIntOrDefault);
    }
    
    /// <summary>
    /// Get the inner text from the element and try to parse it as decimal or return the default int value.
    /// </summary>
    /// <param name="elementHandle"></param>
    /// <returns></returns>
    public static Task<decimal> GetDecimalAsync(this ElementHandle elementHandle)
    {
        return GetInnerTextAsync(elementHandle).AwaitAndModify(RetrieveExtensions.GetDecimalOrDefault);
    }
}