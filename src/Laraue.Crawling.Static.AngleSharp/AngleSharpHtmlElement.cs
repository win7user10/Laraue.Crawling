using AngleSharp.Dom;
using Laraue.Crawling.Abstractions;
using Laraue.Crawling.Static.Abstractions;

namespace Laraue.Crawling.Static.AngleSharp;

/// <summary>
/// AngleSharp wrapper for <see cref="IHtmlElement"/>.
/// </summary>
public class AngleSharpHtmlElement : IHtmlElement
{
    private readonly IElement _element;

    public AngleSharpHtmlElement(IElement element)
    {
        _element = element;
    }

    public string GetInnerHtml()
    {
        return _element.InnerHtml;
    }

    public string? GetAttribute(string name)
    {
        return _element.Attributes.GetNamedItem(name)?.Value;
    }
}