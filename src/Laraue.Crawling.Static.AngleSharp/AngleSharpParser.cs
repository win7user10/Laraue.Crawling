using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using Laraue.Crawling.Abstractions;
using Laraue.Crawling.Common;
using Laraue.Crawling.Static.Abstractions;
using Laraue.Crawling.Static.Impl;

namespace Laraue.Crawling.Static.AngleSharp;

public class AngleSharpParser : BaseHtmlSchemaParser<IElement>
{
    public override async Task<TModel?> ParseAsync<TModel>(
        ICompiledStaticHtmlSchema<IElement, TModel> schema,
        IElement? rootElement)
        where TModel : default
    {
        return (TModel?) await ParseAsync(schema.BindingExpression, rootElement);
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