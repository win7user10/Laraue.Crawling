using Laraue.Crawling.Abstractions;

namespace Laraue.Crawling.Common.Impl;

/// <inheritdoc />
public abstract class BaseCrawlingAdapter<TNode> : ICrawlingAdapter<TNode>
{
    private readonly ValueMapper _valueMapper;

    protected BaseCrawlingAdapter(ValueMapper valueMapper)
    {
        _valueMapper = valueMapper;
    }

    /// <inheritdoc />
    public TDestination? MapValue<TDestination>(string? element)
    {
        return _valueMapper.Map<TDestination>(element);
    }

    /// <inheritdoc />
    public Task<object?> GetValueAsync(TNode? element, Type destinationType)
    {
        return GetValueAsync(element, destinationType, GetInnerTextAsync);
    }

    /// <inheritdoc />
    public abstract Task<string?> GetInnerTextAsync(TNode? element);

    /// <inheritdoc />
    public abstract Task<string?> GetAttributeTextAsync(TNode? element, string attributeName);
    
    private object? MapValue(string? element, Type destinationType)
    {
        return _valueMapper.Map(element, destinationType);
    }

    private async Task<object?> GetValueAsync(TNode? element, Type destinationType, Func<TNode?, Task<string?>> getInnerText)
    {
        var textContent = await getInnerText(element).ConfigureAwait(false);

        return MapValue(textContent, destinationType);
    }
}