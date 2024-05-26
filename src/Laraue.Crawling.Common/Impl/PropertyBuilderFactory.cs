using Laraue.Crawling.Abstractions;

namespace Laraue.Crawling.Common.Impl;

public abstract class PropertyBuilderFactory<TElement>
{
    private readonly ICrawlingAdapter<TElement> _crawlingAdapter;

    public PropertyBuilderFactory(ICrawlingAdapter<TElement> crawlingAdapter)
    {
        _crawlingAdapter = crawlingAdapter;
    }
    
    public PropertyBuilder<TElement, TSelector, TModel, TValue> GetPropertyBuilder<TSelector, TModel, TValue>()
        where TSelector : Selector
        where TModel : class, ICrawlingModel
    {
        return new PropertyBuilder<TElement, TSelector, TModel, TValue>(_crawlingAdapter);
    }
}