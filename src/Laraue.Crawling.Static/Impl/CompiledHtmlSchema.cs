using Laraue.Crawling.Static.Abstractions;

namespace Laraue.Crawling.Static.Impl;

internal class CompiledHtmlSchema<TModel> : ICompiledHtmlSchema<TModel>
{
    public CompiledHtmlSchema(ComplexTypeBindingExpression bindingExpression)
    {
        BindingExpression = bindingExpression;
    }
    
    public ComplexTypeBindingExpression BindingExpression { get; }
}