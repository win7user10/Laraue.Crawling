using Laraue.Crawling.Abstractions;

namespace Laraue.Crawling.Common.Impl;

public abstract class BaseExtractors<TElement> : IExtractors<TElement>
{
    private readonly ValueMapper _valueMapper;

    protected BaseExtractors(ValueMapper valueMapper)
    {
        _valueMapper = valueMapper;
    }

    /// <inheritdoc />
    public TDestination? MapValue<TDestination>(string? element)
    {
        return _valueMapper.Map<TDestination>(element);
    }

    /// <inheritdoc />
    public object? MapValue(string? element, Type destinationType)
    {
        return _valueMapper.Map(element, destinationType);
    }

    /// <inheritdoc />
    public async Task<object?> GetValueAsync(TElement? element, Type destinationType, Func<TElement?, Task<string?>> getInnerText)
    {
        var textContent = await getInnerText(element).ConfigureAwait(false);

        return MapValue(textContent, destinationType);
    }

    /// <inheritdoc />
    public Task<object?> GetValueAsync(TElement? element, Type destinationType)
    {
        return GetValueAsync(element, destinationType, GetInnerTextAsync);
    }

    /// <inheritdoc />
    public async Task<TValue?> GetValueAsync<TValue>(TElement? element, Func<TElement?, Task<string?>> getInnerText)
    {
        return (TValue?) await GetValueAsync(element, typeof(TValue), getInnerText);
    }

    /// <inheritdoc />
    public abstract Task<string?> GetInnerTextAsync(TElement? element);

    /// <inheritdoc />
    public abstract Task<string?> GetAttributeTextAsync(TElement? element, string attributeName);

    /// <inheritdoc />
    public Func<TElement, Task<TValue?>> GetValueFromInnerTextAsync<TValue>(Func<string?, Task<TValue?>> getValueFromInnerText)
    {
        return async element =>
        {
            var innerText = await GetInnerTextAsync(element).ConfigureAwait(false);
            return await getValueFromInnerText(innerText).ConfigureAwait(false);
        };
    }
}

public interface IExtractors<TElement>
{
    public TDestination? MapValue<TDestination>(string? element);
    public object? MapValue(string? element, Type destinationType);
    public Task<object?> GetValueAsync(TElement? element, Type destinationType, Func<TElement?, Task<string?>> getInnerText);
    public Task<object?> GetValueAsync(TElement? element, Type destinationType);
    public Task<TValue?> GetValueAsync<TValue>(TElement? element, Func<TElement?, Task<string?>> getInnerText);
    public Task<string?> GetInnerTextAsync(TElement? element);
    public Task<string?> GetAttributeTextAsync(TElement? element, string attributeName);
    public Func<TElement, Task<TValue?>> GetValueFromInnerTextAsync<TValue>(Func<string?, Task<TValue?>> getValueFromInnerText);
}

public class PropertyBuilder<TElement, TSelector, TModel, TValue>
    where TSelector : Selector
    where TModel : class, ICrawlingModel
{
    internal TSelector? Selector { get; private set; }
    internal IExtractors<TElement> Extractors { get; }

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

    public PropertyBuilder(IExtractors<TElement> extractors)
    {
        Extractors = extractors;

        GetInnerTextAsync = Extractors.GetInnerTextAsync;
        MapValue = Extractors.MapValue<TValue>;
        
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
        GetInnerTextAsync = element => Extractors.GetAttributeTextAsync(element, attributeName);
        
        return this;
    }
}