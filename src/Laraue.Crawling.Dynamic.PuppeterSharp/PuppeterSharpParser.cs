using Laraue.Crawling.Abstractions;
using Laraue.Crawling.Common.Impl;
using Microsoft.Extensions.Logging;
using PuppeteerSharp;

namespace Laraue.Crawling.Dynamic.PuppeterSharp;

/// <summary>
/// The implementation of the crawler based on the
/// <see href="https://github.com/hardkoded/puppeteer-sharp">PupeeterSharp</see> library.
/// </summary>
public class PuppeterSharpParser : BaseHtmlSchemaParser<IElementHandle>
{
    /// <summary>
    /// Initializes a new instance of <see cref="PuppeterSharpParser"/>.
    /// </summary>
    /// <param name="loggerFactory"></param>
    public PuppeterSharpParser(ILoggerFactory loggerFactory)
        : base(loggerFactory)
    {
    }
    
    /// <inheritdoc />
    protected override Task<IElementHandle?> GetElementAsync(
        IElementHandle currentElement,
        HtmlSelector htmlSelector)
    {
        return currentElement.QuerySelectorAsync(htmlSelector.Selector);
    }

    /// <inheritdoc />
    protected override Task<IElementHandle[]?> GetElementsAsync(
        IElementHandle currentElement,
        HtmlSelector htmlSelector)
    {
        return currentElement.QuerySelectorAllAsync(htmlSelector.Selector);
    }
}