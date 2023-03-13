namespace Laraue.Crawling.Abstractions;

/// <summary>
/// <see cref="ICompiledHtmlSchema{TElement,TModel}"/> parser.
/// </summary>
public interface IHtmlSchemaParser<TElement>
{
    /// <summary>
    /// Parse passed schema and return the result.
    /// </summary>
    /// <param name="schema"></param>
    /// <param name="rootElement"></param>
    /// <typeparam name="TModel"></typeparam>
    /// <returns></returns>
    public Task<TModel?> RunAsync<TModel>(
        ICompiledHtmlSchema<TElement, TModel> schema,
        TElement? rootElement);
}