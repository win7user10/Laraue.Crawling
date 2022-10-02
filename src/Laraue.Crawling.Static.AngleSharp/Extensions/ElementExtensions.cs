using AngleSharp.Dom;

namespace Laraue.Crawling.Static.AngleSharp.Extensions;

public static class ElementExtensions
{
    public static string? GetAttributeValue(this IElement? element, string attribute)
    {
        return element?.Attributes?.GetNamedItem(attribute)?.Value;
    }
}