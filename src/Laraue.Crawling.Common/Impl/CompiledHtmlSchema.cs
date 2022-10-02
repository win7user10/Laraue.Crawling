using Laraue.Crawling.Abstractions;
using Laraue.Crawling.Abstractions.Schema.Binding;

namespace Laraue.Crawling.Common.Impl;

internal class CompiledHtmlSchema<TElement, TModel> : ICompiledHtmlSchema<TElement, TModel>
{
    public CompiledHtmlSchema(BindObjectExpression<TElement> bindingExpression)
    {
        BindingExpression = bindingExpression;
    }
    
    public BindObjectExpression<TElement> BindingExpression { get; }
}