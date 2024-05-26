using System.Xml;
using Laraue.Crawling.Abstractions;
using Laraue.Crawling.Common.Impl;
using Microsoft.Extensions.Logging;

namespace Laraue.Crawling.Static.Xml;

/// <inheritdoc />
public class XmlParser : BaseDocumentSchemaParser<XmlNode, XPathSelector>
{
    /// <inheritdoc />
    public XmlParser(ILoggerFactory loggerFactory) : base(loggerFactory)
    {
    }
    
    /// <inheritdoc />
    protected override Task<XmlNode?> GetElementAsync(XmlNode currentElement, XPathSelector htmlSelector)
    {
        return Task.FromResult(currentElement?.SelectSingleNode(htmlSelector.Value));
    }

    /// <inheritdoc />
    protected override Task<XmlNode[]?> GetElementsAsync(XmlNode currentElement, XPathSelector htmlSelector)
    {
        var result = currentElement?.SelectNodes(htmlSelector.Value);

        return Task.FromResult(result?.Cast<XmlNode>().ToArray());
    }
}