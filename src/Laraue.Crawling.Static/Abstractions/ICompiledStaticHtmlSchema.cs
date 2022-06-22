namespace Laraue.Crawling.Static.Abstractions;

/// <summary>
/// Schema ready for the crawling.
/// </summary>
/// <typeparam name="TModel"></typeparam>
public interface ICompiledStaticHtmlSchema<in TModel>
{
    public ComplexTypeBindingExpression BindingExpression { get; }
}