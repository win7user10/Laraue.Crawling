using Laraue.Crawling.Static.Abstractions;

namespace Laraue.Crawling.Static.Impl;

internal class CompiledStaticHtmlSchema<TModel> : ICompiledStaticHtmlSchema<TModel>
{
    public CompiledStaticHtmlSchema(ComplexTypeBindingExpression bindingExpression)
    {
        BindingExpression = bindingExpression;
    }
    
    public ComplexTypeBindingExpression BindingExpression { get; }
}