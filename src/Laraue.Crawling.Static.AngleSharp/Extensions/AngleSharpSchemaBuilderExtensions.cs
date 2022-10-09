using System.Linq.Expressions;
using System.Text.Json;
using AngleSharp.Dom;
using Laraue.Crawling.Abstractions;
using Laraue.Crawling.Common.Impl;

namespace Laraue.Crawling.Static.AngleSharp.Extensions;

public static class AngleSharpSchemaBuilderExtensions
{
    /// <summary>
    /// Use InnerHtml to bind the property.
    /// </summary>
    /// <param name="schemaBuilder"></param>
    /// <param name="schemaProperty"></param>
    /// <param name="htmlSelector"></param>
    /// <param name="modifyFunc"></param>
    /// <typeparam name="TModel"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    public static HtmlSchemaBuilder<IElement, TModel> HasProperty<TModel, TValue>(
        this HtmlSchemaBuilder<IElement, TModel> schemaBuilder,
        Expression<Func<TModel, TValue?>> schemaProperty,
        HtmlSelector htmlSelector,
        Func<string, string>? modifyFunc = null)
    {
        return schemaBuilder.HasProperty(
            schemaProperty,
            htmlSelector,
            element =>
            {
                var textContent = element.TextContent;

                if (modifyFunc is not null)
                {
                    textContent = modifyFunc(textContent);
                }
                
                var value = typeof(TValue) == typeof(string)
                    ? (dynamic) textContent
                    : JsonSerializer.Deserialize<TValue>(textContent);

                return Task.FromResult(value);
            });
    }
}