using Laraue.Crawling.Abstractions;
using Laraue.Crawling.Dynamic.PuppeterSharp.Abstractions;
using PuppeteerSharp;

namespace Laraue.Crawling.Dynamic.PuppeterSharp;

/// <inheritdoc />
public sealed class PageParser : IPageParser
{
    private readonly IDocumentSchemaParser<IElementHandle, HtmlSelector> _schemaParser;
    
    public PageParser(IDocumentSchemaParser<IElementHandle, HtmlSelector> schemaParser)
    {
        _schemaParser = schemaParser;
    }
    
    /// <inheritdoc />
    public async Task<TResult> ParseAsync<TResult>(IPage page, ICompiledDocumentSchema<IElementHandle, HtmlSelector, TResult> schema)
    {
        var element = await page.QuerySelectorAsync("body")
            .ConfigureAwait(false);
        
        return await _schemaParser.RunAsync(schema, element).ConfigureAwait(false)
               ?? throw new InvalidOperationException("Tag <body> has not been found in the passed page");
    }
}