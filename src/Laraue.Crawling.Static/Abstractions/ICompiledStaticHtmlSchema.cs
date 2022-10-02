using Laraue.Crawling.Abstractions.Schema;

namespace Laraue.Crawling.Static.Abstractions;

/// <summary>
/// Schema ready for the crawling.
/// </summary>
/// <typeparam name="TModel"></typeparam>
/// <typeparam name="TElement"></typeparam>
public interface ICompiledStaticHtmlSchema<TElement, in TModel>
{
    /// <summary>
    /// Schema of the parsing for the current object.
    /// </summary>
    public BindObjectExpression<TElement> BindingExpression { get; }
}