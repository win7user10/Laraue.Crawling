using System.Xml;
using Laraue.Crawling.Abstractions;
using Laraue.Crawling.Common.Impl;

namespace Laraue.Crawling.Static.Xml;

/// <inheritdoc />
public class XmlSchemaBuilder<TModel> : DocumentSchemaBuilder<XmlNode, XPathSelector, TModel>
    where TModel : class, ICrawlingModel
{
    /// <inheritdoc />
    public XmlSchemaBuilder()
        : base(new XmlPropertyBuilderFactory())
    {
    }
}