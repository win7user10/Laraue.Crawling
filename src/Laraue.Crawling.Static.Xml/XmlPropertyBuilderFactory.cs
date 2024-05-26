using System.Xml;
using Laraue.Crawling.Common.Impl;

namespace Laraue.Crawling.Static.Xml;

/// <inheritdoc />
public class XmlPropertyBuilderFactory : PropertyBuilderFactory<XmlNode>
{
    /// <inheritdoc />
    public XmlPropertyBuilderFactory()
        : base(new XmlCrawlingAdapter(new JsonValueMapper()))
    {
    }
}

/// <inheritdoc />
public class XmlCrawlingAdapter : BaseCrawlingAdapter<XmlNode>
{
    /// <inheritdoc />
    public XmlCrawlingAdapter(ValueMapper valueMapper) : base(valueMapper)
    {
    }

    /// <inheritdoc />
    public override Task<string?> GetInnerTextAsync(XmlNode? element)
    {
        return Task.FromResult(element?.InnerText);
    }

    /// <inheritdoc />
    public override Task<string?> GetAttributeTextAsync(XmlNode? element, string attributeName)
    {
        return Task.FromResult(element?.Attributes?.GetNamedItem(attributeName)?.InnerText);
    }
}