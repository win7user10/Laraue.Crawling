using Laraue.Crawling.Abstractions.Schema;
using Laraue.Crawling.Static.Abstractions;

namespace Laraue.Crawling.Static.Impl;

internal class CompiledStaticHtmlSchema<TElement, TModel> : ICompiledStaticHtmlSchema<TElement, TModel>
{
    public CompiledStaticHtmlSchema(BindObjectExpression<TElement> bindingExpression)
    {
        BindingExpression = bindingExpression;
    }
    
    public BindObjectExpression<TElement> BindingExpression { get; }
}