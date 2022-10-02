namespace Laraue.Crawling.Abstractions;

/// <summary>
/// <see cref="ICompiledHtmlSchema{TElement,TModel}"/> parser.
/// </summary>
public interface IHtmlSchemaParser<TElement>
{
    /// <summary>
    /// Returns the model from the passed schema and html code.
    /// </summary>
    /// <param name="schema"></param>
    /// <param name="html"></param>
    /// <typeparam name="TModel"></typeparam>
    /// <returns></returns>
    public TModel? Parse<TModel>(ICompiledHtmlSchema<TElement, TModel> schema, string html);
}