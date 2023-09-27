using System.Xml;
using Laraue.Crawling.Abstractions;
using Laraue.Crawling.Common.Impl;

namespace Laraue.Crawling.Static.Xml;

public class XmlSchemaBuilder<TModel> : DocumentSchemaBuilder<XmlNode, XPathSelector, TModel>
{
}