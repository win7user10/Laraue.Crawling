using System.Xml;
using Laraue.Crawling.Abstractions;
using Laraue.Crawling.Common.Impl;
using Microsoft.Extensions.Logging;

namespace Laraue.Crawling.Static.Xml;

public class XmlParser : BaseDocumentSchemaParser<XmlNode, XPathSelector>
{
    public XmlParser(ILoggerFactory loggerFactory) : base(loggerFactory)
    {
    }
    
    protected override Task<XmlNode?> GetElementAsync(XmlNode currentElement, XPathSelector htmlSelector)
    {
        return Task.FromResult(currentElement?.SelectSingleNode(htmlSelector.Value));
    }

    protected override Task<XmlNode[]?> GetElementsAsync(XmlNode currentElement, XPathSelector htmlSelector)
    {
        var result = currentElement?.SelectNodes(htmlSelector.Value);

        return Task.FromResult(result?.Cast<XmlNode>().ToArray());
    }
}