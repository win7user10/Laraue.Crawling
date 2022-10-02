using AngleSharp.Dom;
using Laraue.Crawling.Abstractions;
using Laraue.Crawling.Common.Impl;

namespace Laraue.Crawling.Static.AngleSharp;

public class AngleSharpParser : BaseHtmlSchemaParser<IElement>
{
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