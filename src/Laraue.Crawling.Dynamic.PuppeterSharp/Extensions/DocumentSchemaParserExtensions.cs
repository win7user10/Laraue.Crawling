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
    
    /// <summary>
    /// Take the opened page, use the passed schema and returns the crawling result.
    /// </summary>
    public static async Task<TResult?> ParseAsync<TResult>(
        this IDocumentSchemaParser<IElementHandle, HtmlSelector> parser,
        IPage page,
        ICompiledElementSchema<IElementHandle, HtmlSelector, TResult> schema)
    {
        var result = await ParseAsync(parser, page, schema.ObjectSchema).ConfigureAwait(false);

        return result.Value;
    }
}