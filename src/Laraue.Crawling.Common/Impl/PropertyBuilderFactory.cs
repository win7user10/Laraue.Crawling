using Laraue.Crawling.Abstractions;

namespace Laraue.Crawling.Common.Impl;

public abstract class PropertyBuilderFactory<TElement>
{
    private readonly IExtractors<TElement> _extractors;

    public PropertyBuilderFactory(IExtractors<TElement> extractors)
    {
        _extractors = extractors;
    }
    
    public PropertyBuilder<TElement, TSelector, TModel, TValue> GetPropertyBuilder<TSelector, TModel, TValue>()
        where TSelector : Selector
        where TModel : class, ICrawlingModel
    {
        return new PropertyBuilder<TElement, TSelector, TModel, TValue>(_extractors);
    }
}