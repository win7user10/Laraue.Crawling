namespace Laraue.Crawling.Static.Abstractions;

/// <summary>
/// <see cref="ICompiledStaticHtmlSchema{TElement, TModel}"/> parser.
/// </summary>
public interface IStaticHtmlSchemaParser<TElement>
{
    /// <summary>
    /// Returns the model from the passed schema and html code.
    /// </summary>
    /// <param name="schema"></param>
    /// <param name="html"></param>
    /// <typeparam name="TModel"></typeparam>
    /// <returns></returns>
    public TModel? Parse<TModel>(ICompiledStaticHtmlSchema<TElement, TModel> schema, string html);
}