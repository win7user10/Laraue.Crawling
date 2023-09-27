namespace Laraue.Crawling.Abstractions;

/// <summary>
/// <see cref="ICompiledDocumentSchema{TElement,TModel,TSelector}"/> parser.
/// </summary>
public interface IDocumentSchemaParser<TElement, TSelector>
    where TSelector : Selector
{
    /// <summary>
    /// Parse passed schema and return the result.
    /// </summary>
    /// <param name="schema"></param>
    /// <param name="rootElement"></param>
    /// <typeparam name="TModel"></typeparam>
    /// <returns></returns>
    public Task<TModel?> RunAsync<TModel>(
        ICompiledDocumentSchema<TElement, TSelector, TModel> schema,
        TElement? rootElement);
}