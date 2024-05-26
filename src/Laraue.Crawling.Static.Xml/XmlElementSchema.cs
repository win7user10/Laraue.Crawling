using System.Xml;
using Laraue.Crawling.Abstractions;
using Laraue.Crawling.Common.Impl;

namespace Laraue.Crawling.Static.Xml;

/// <inheritdoc />
public class XmlElementSchema<TModel> : ElementSchema<XmlNode, XPathSelector, TModel?>
{
    /// <inheritdoc />
    public XmlElementSchema(Action<PropertyBuilder<XmlNode, XPathSelector, GenericCrawlingModel<TModel?>, TModel?>> propertyBuilder)
        : base(new XmlSchemaBuilder<GenericCrawlingModel<TModel?>>(), propertyBuilder)
    {
    }
}