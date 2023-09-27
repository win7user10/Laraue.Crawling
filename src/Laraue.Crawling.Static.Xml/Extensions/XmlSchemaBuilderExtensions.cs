using System.Linq.Expressions;
using System.Text.Json;
using System.Xml;
using Laraue.Crawling.Abstractions;
using Laraue.Crawling.Common;
using Laraue.Crawling.Common.Impl;

namespace Laraue.Crawling.Static.Xml.Extensions;

public static class XmlSchemaBuilderExtensions
{
    /// <summary>
    /// Use InnerText to bind the property.
    /// </summary>
    /// <param name="schemaBuilder"></param>
    /// <param name="schemaProperty"></param>
    /// <param name="selector"></param>
    /// <param name="modifyFunc"></param>
    /// <typeparam name="TModel"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    public static DocumentSchemaBuilder<XmlNode, XPathSelector, TModel> HasProperty<TModel, TValue>(
        this DocumentSchemaBuilder<XmlNode, XPathSelector, TModel> schemaBuilder,
        Expression<Func<TModel, TValue?>> schemaProperty,
        XPathSelector? selector = null,
        Func<string, string>? modifyFunc = null)
    {
        return schemaBuilder.HasProperty(
            schemaProperty,
            selector,
            element =>
            {
                var textContent = element.InnerText;

                if (modifyFunc is not null)
                {
                    textContent = modifyFunc(textContent);
                }
                
                return Task.FromResult(StringValueMapper.Map<TValue>(textContent));
            });
    }
}