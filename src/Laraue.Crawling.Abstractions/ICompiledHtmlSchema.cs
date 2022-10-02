using Laraue.Crawling.Abstractions.Schema.Binding;

namespace Laraue.Crawling.Abstractions;

/// <summary>
/// Schema ready for the crawling.
/// </summary>
/// <typeparam name="TModel"></typeparam>
/// <typeparam name="TElement"></typeparam>
public interface ICompiledHtmlSchema<TElement, in TModel>
{
    /// <summary>
    /// Schema of the parsing for the current object.
    /// </summary>
    public BindObjectExpression<TElement> BindingExpression { get; }
}