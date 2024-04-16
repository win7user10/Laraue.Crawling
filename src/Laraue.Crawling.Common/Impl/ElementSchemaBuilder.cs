using Laraue.Crawling.Abstractions;
using Laraue.Crawling.Abstractions.Schema;

namespace Laraue.Crawling.Common.Impl;

public class ElementSchemaBuilder<TElement, TSelector, TModel>
    where TSelector : Selector
{
    public ICompiledElementSchema<TElement, TSelector, TModel> HasValue(Func<TElement, Task<TModel>> function, TSelector selector)
    {
        return new ElementSchema<TElement, TSelector, TModel>(
            new ReturnExpression<TElement, TSelector>(
                async element => await function.Invoke(element).ConfigureAwait(false),
                selector));
    }
}

public class ElementSchema<TElement, TSelector, TModel> : ICompiledElementSchema<TElement, TSelector, TModel>
    where TSelector : Selector
{
    public ElementSchema(ReturnExpression<TElement, TSelector> bindingExpression)
    {
        BindingExpression = bindingExpression;
    }

    public ReturnExpression<TElement, TSelector> BindingExpression { get; }
}