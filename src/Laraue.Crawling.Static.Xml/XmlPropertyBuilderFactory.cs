using System.Xml;
using Laraue.Crawling.Common.Impl;

namespace Laraue.Crawling.Static.Xml;

public class XmlPropertyBuilderFactory : PropertyBuilderFactory<XmlNode>
{
    public XmlPropertyBuilderFactory()
        : base(new XmlExtractors(new JsonValueMapper()))
    {
    }
}

public class XmlExtractors : BaseExtractors<XmlNode>
{
    public XmlExtractors(ValueMapper valueMapper) : base(valueMapper)
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