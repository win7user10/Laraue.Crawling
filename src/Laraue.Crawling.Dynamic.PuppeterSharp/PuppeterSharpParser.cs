using Laraue.Crawling.Abstractions;
using Laraue.Crawling.Common.Impl;
using PuppeteerSharp;

namespace Laraue.Crawling.Dynamic.PuppeterSharp;

public class PuppeterSharpParser : BaseHtmlSchemaParser<ElementHandle>
{
    protected override async Task<ElementHandle?> GetElementAsync(
        ElementHandle currentElement,
        HtmlSelector htmlSelector)
    {
        return await currentElement.QuerySelectorAsync(htmlSelector.Selector).ConfigureAwait(false);
    }

    protected override async Task<ElementHandle[]?> GetElementsAsync(
        ElementHandle currentElement,
        HtmlSelector htmlSelector)
    {
        return await currentElement.QuerySelectorAllAsync(htmlSelector.Selector).ConfigureAwait(false);
    }
}