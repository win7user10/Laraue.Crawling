using Laraue.Crawling.Abstractions;
using PuppeteerSharp;

namespace Laraue.Crawling.Dynamic.PuppeterSharp.Extensions;

/// <summary>
/// Transformers from opened browser page to the crawling model.
/// </summary>
public static class DocumentSchemaParserExtensions
{
    /// <summary>
    /// Take the opened page, use the passed schema and returns the crawling result.
    /// </summary>
    /// <param name="parser"></param>
    /// <param name="page"></param>
    /// <param name="schema"></param>
    /// <typeparam name="TResult"></typeparam>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static async Task<TResult> ParseAsync<TResult>(
        this IDocumentSchemaParser<IElementHandle, HtmlSelector> parser,
        IPage page,
        ICompiledDocumentSchema<IElementHandle, HtmlSelector, TResult> schema)
    {
        var element = await page.QuerySelectorAsync("body")
            .ConfigureAwait(false);
        
        return await parser.RunAsync(schema, element).ConfigureAwait(false)
               ?? throw new InvalidOperationException("Tag <body> has not been found in the passed page");
    }
}