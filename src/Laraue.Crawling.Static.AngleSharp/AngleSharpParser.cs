using AngleSharp.Dom;
using Laraue.Crawling.Abstractions;
using Laraue.Crawling.Common.Impl;
using Microsoft.Extensions.Logging;

namespace Laraue.Crawling.Static.AngleSharp;

public class AngleSharpParser : BaseHtmlSchemaParser<IElement>
{
    public AngleSharpParser(ILoggerFactory loggerFactory) : base(loggerFactory)
    {
    }
    
    protected override Task<IElement?> GetElementAsync(IElement currentElement, HtmlSelector htmlSelector)
    {
        return Task.FromResult(currentElement?.QuerySelector(htmlSelector.Selector));
    }

    protected override Task<IElement[]?> GetElementsAsync(IElement currentElement, HtmlSelector htmlSelector)
    {
        var result = currentElement?.QuerySelectorAll(htmlSelector.Selector);

        return Task.FromResult(result?.ToArray());
    }
}