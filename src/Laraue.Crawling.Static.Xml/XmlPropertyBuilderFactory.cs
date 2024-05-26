using System.Xml;
using Laraue.Crawling.Common.Impl;

namespace Laraue.Crawling.Static.Xml;

public class XmlPropertyBuilderFactory : PropertyBuilderFactory<XmlNode>
{
    public XmlPropertyBuilderFactory()
        : base(new XmlCrawlingAdapter(new JsonValueMapper()))
    {
    }
}

public class XmlCrawlingAdapter : BaseCrawlingAdapter<XmlNode>
{
    public XmlCrawlingAdapter(ValueMapper valueMapper) : base(valueMapper)
    {
    }

    public override Task<string?> GetInnerTextAsync(XmlNode? element)
    {
        return Task.FromResult(element?.InnerText);
    }

    public override Task<string?> GetAttributeTextAsync(XmlNode? element, string attributeName)
    {
        return Task.FromResult(element?.Attributes?.GetNamedItem(attributeName)?.InnerText);
    }
}