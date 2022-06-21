namespace Laraue.Crawling.Static.Abstractions;


public interface ICompiledHtmlSchema<in TModel>
{
    public ComplexTypeBindingExpression BindingExpression { get; }
}