using Laraue.Crawling.Abstractions;
using Laraue.Crawling.Common.Impl;
using Microsoft.Extensions.Logging;
using PuppeteerSharp;

namespace Laraue.Crawling.Dynamic.PuppeterSharp;

public class PuppeterSharpParser : BaseHtmlSchemaParser<ElementHandle>
{
    public PuppeterSharpParser(ILoggerFactory loggerFactory) : base(loggerFactory)
    {
    }
    
    protected override Task<ElementHandle?> GetElementAsync(
        ElementHandle currentElement,
        HtmlSelector htmlSelector)
    {
        return currentElement.QuerySelectorAsync(htmlSelector.Selector);
    }

    protected override Task<ElementHandle[]?> GetElementsAsync(
        ElementHandle currentElement,
        HtmlSelector htmlSelector)
    {
        return currentElement.QuerySelectorAllAsync(htmlSelector.Selector);
    }
}