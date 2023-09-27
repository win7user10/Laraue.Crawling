using Laraue.Crawling.Abstractions;
using Laraue.Crawling.Abstractions.Schema.Binding;

namespace Laraue.Crawling.Common.Impl;

/// <summary>
/// The schema which can be used for the parsing.
/// </summary>
/// <typeparam name="TElement"></typeparam>
/// <typeparam name="TSelector"></typeparam>
/// <typeparam name="TModel"></typeparam>
public class CompiledDocumentSchema<TElement, TSelector, TModel>
    : ICompiledDocumentSchema<TElement, TSelector, TModel>
    where TSelector : Selector
{
    /// <summary>
    /// Initializes a new instance of <see cref="CompiledDocumentSchema{TElement,TModel,TSelector}"/>.
    /// Can be used to create strongly-types schema as a class.
    /// </summary>
    /// <param name="bindingExpression"></param>
    public CompiledDocumentSchema(BindObjectExpression<TElement, TSelector> bindingExpression)
    {
        BindingExpression = bindingExpression;
    }

    /// <summary>
    /// Covert schema to the BindObjectExpression.
    /// </summary>
    /// <param name="schema"></param>
    /// <returns></returns>
    public static implicit operator BindObjectExpression<TElement, TSelector>(
        CompiledDocumentSchema<TElement, TSelector, TModel> schema)
    {
        return schema.BindingExpression;
    }
    
    /// <summary>
    /// Root binding expression.
    /// </summary>
    public BindObjectExpression<TElement, TSelector> BindingExpression { get; }
}