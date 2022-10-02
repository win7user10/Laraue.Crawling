using System.Linq.Expressions;
using System.Text.Json;
using AngleSharp.Dom;
using Laraue.Crawling.Abstractions;
using Laraue.Crawling.Static.Impl;

namespace Laraue.Crawling.Static.AngleSharp;

public static class AngleSharpSchemaBuilderExtensions
{
    /// <summary>
    /// Use InnerHtml to bind the property.
    /// </summary>
    /// <param name="schemaBuilder"></param>
    /// <param name="schemaProperty"></param>
    /// <param name="htmlSelector"></param>
    /// <typeparam name="TModel"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    public static StaticHtmlSchemaBuilder<IElement, TModel> HasProperty<TModel, TValue>(
        this StaticHtmlSchemaBuilder<IElement, TModel> schemaBuilder,
        Expression<Func<TModel, TValue>> schemaProperty,
        HtmlSelector htmlSelector)
    {
        return schemaBuilder.HasProperty(
            schemaProperty,
            htmlSelector,
            element =>
            {
                var value = typeof(TValue) == typeof(string)
                    ? (dynamic) element.InnerHtml
                    : JsonSerializer.Deserialize<TValue>(element.InnerHtml);

                return Task.FromResult(value);
            });
    }
}