using Laraue.Crawling.Abstractions;

namespace Laraue.Crawling.Static.Xml;

/// <inheritdoc />
public record XPathSelector : Selector
{
    /// <inheritdoc />
    public XPathSelector(string value)
        : base(value)
    {
    }

    public static implicit operator XPathSelector(string value)
    {
        return new XPathSelector(value);
    }
}