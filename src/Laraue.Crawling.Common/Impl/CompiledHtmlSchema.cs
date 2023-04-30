using Laraue.Crawling.Abstractions;
using Laraue.Crawling.Abstractions.Schema.Binding;

namespace Laraue.Crawling.Common.Impl;

/// <summary>
/// The schema which can be used for the parsing.
/// </summary>
/// <typeparam name="TElement"></typeparam>
/// <typeparam name="TModel"></typeparam>
public class CompiledHtmlSchema<TElement, TModel> : ICompiledHtmlSchema<TElement, TModel>
{
    /// <summary>
    /// Initializes a new instance of <see cref="CompiledHtmlSchema{TElement, TModel}"/>.
    /// Can be used to create strongly-types schema as a class.
    /// <example>
    ///     public class MySchema : ICompiledHtmlSchema&lt;IElement, CianAdvertisementBatch&gt;
    ///     {
    ///         public MySchema(Dependency dependency) : base(GetSchema(dependency))
    ///         {
    ///         }
    ///
    ///         public ICompiledHtmlSchema&lt;IElement, CianAdvertisementBatch&gt; GetSchema(Dependency dependency)
    ///         {
    ///             return new PuppeterSharpSchemaBuilder&lt;CianAdvertisementBatch&gt;().Build();
    ///         }
    ///     }
    /// </example>
    /// </summary>
    /// <param name="bindingExpression"></param>
    public CompiledHtmlSchema(BindObjectExpression<TElement> bindingExpression)
    {
        BindingExpression = bindingExpression;
    }

    /// <summary>
    /// Covert schema to the BindObjectExpression.
    /// </summary>
    /// <param name="schema"></param>
    /// <returns></returns>
    public static implicit operator BindObjectExpression<TElement>(
        CompiledHtmlSchema<TElement, TModel> schema)
    {
        return schema.BindingExpression;
    }
    
    /// <summary>
    /// Root binding expression.
    /// </summary>
    public BindObjectExpression<TElement> BindingExpression { get; }
}