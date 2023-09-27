using Laraue.Crawling.Abstractions.Schema.Binding;

namespace Laraue.Crawling.Abstractions;

/// <summary>
/// Schema ready for the crawling.
/// </summary>
/// <typeparam name="TModel"></typeparam>
/// <typeparam name="TSelector"></typeparam>
/// <typeparam name="TElement"></typeparam>
public interface ICompiledDocumentSchema<TElement, TSelector, in TModel>
    where TSelector : Selector
{
    /// <summary>
    /// Schema of the parsing for the current object.
    /// </summary>
    public BindObjectExpression<TElement, TSelector> BindingExpression { get; }
}