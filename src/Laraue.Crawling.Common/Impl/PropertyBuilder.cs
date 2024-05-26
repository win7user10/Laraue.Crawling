using Laraue.Crawling.Abstractions;

namespace Laraue.Crawling.Common.Impl;

public class PropertyBuilder<TElement, TSelector, TModel, TValue>
    where TSelector : Selector
    where TModel : class, ICrawlingModel
{
    internal TSelector? Selector { get; private set; }
    internal ICrawlingAdapter<TElement> CrawlingAdapter { get; }

    /// <summary>
    /// Default delegate to get text content from an element.
    /// </summary>
    private Func<TElement?, Task<string?>> GetInnerTextAsync { get; set; }
    
    /// <summary>
    /// Default delegate to map text content to the specified value type.
    /// </summary>
    private Func<string?, TValue?> MapValue { get; set; }

    /// <summary>
    /// <see cref="ExecuteDefaultDelegateAsync"/> as default or custom delegate.
    /// </summary>
    internal Func<TElement, Task<TValue?>> GetValueAsyncDelegate { get; private set; }

    public PropertyBuilder(ICrawlingAdapter<TElement> crawlingAdapter)
    {
        CrawlingAdapter = crawlingAdapter;

        GetInnerTextAsync = CrawlingAdapter.GetInnerTextAsync;
        MapValue = CrawlingAdapter.MapValue<TValue>;
        
        GetValueAsyncDelegate = ExecuteDefaultDelegateAsync;
    }

    private async Task<TValue?> ExecuteDefaultDelegateAsync(TElement? element)
    {
        var innerText = await GetInnerTextAsync(element).ConfigureAwait(false);
        return MapValue(innerText);
    }
    
    public void GetValueFromElement(Func<TElement?, Task<TValue?>> getValueAsyncDelegate)
    {
        GetValueAsyncDelegate = getValueAsyncDelegate;
    }
    
    public PropertyBuilder<TElement, TSelector, TModel, TValue> Map(Func<string?, TValue?> getValueDelegate)
    {
        MapValue = getValueDelegate;

        return this;
    }
    
    public void GetValueFromElement(Func<TElement?, TValue?> getValueDelegate)
    {
        GetValueFromElement(element => Task.FromResult(getValueDelegate(element)));
    }
    
    public PropertyBuilder<TElement, TSelector, TModel, TValue> UseSelector(TSelector selector)
    {
        Selector = selector;
        
        return this;
    }
    
    public PropertyBuilder<TElement, TSelector, TModel, TValue> GetInnerTextFromAttribute(string attributeName)
    {
        GetInnerTextAsync = element => CrawlingAdapter.GetAttributeTextAsync(element, attributeName);
        
        return this;
    }
}