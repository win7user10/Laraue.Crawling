using System.Linq.Expressions;
using System.Text.Json;
using Laraue.Crawling.Abstractions;
using Laraue.Crawling.Common.Extensions;
using Laraue.Crawling.Common.Impl;
using PuppeteerSharp;

namespace Laraue.Crawling.Dynamic.PuppeterSharp;

public static class PuppeterSharpSchemaBuilderExtensions
{
    public static HtmlSchemaBuilder<IElementHandle, TModel> HasProperty<TModel>(
        this HtmlSchemaBuilder<IElementHandle, TModel> schema,
        Expression<Func<TModel, string?>> schemaProperty,
        HtmlSelector htmlSelector)
    {
        return schema.HasProperty(schemaProperty, htmlSelector, element =>
        {
            if (element is null)
            {
                throw new Exception($"Handle {schemaProperty} error. The element is null.");
            }

            return element.GetTrimmedInnerTextAsync();
        });
    }
    
    public static HtmlSchemaBuilder<IElementHandle, TModel> HasProperty<TModel, TValue>(
        this HtmlSchemaBuilder<IElementHandle, TModel> schemaBuilder,
        Expression<Func<TModel, TValue?>> schemaProperty,
        HtmlSelector htmlSelector,
        Func<string, string>? modifyFunc = null)
    {
        return schemaBuilder.HasProperty<TValue>(
            schemaProperty,
            htmlSelector,
            async element =>
            {
                var textContent = await element.GetInnerTextAsync();
                if (textContent is null)
                {
                    return default;
                }

                if (modifyFunc is not null)
                {
                    textContent = modifyFunc(textContent);
                }
                
                var value = typeof(TValue) == typeof(string)
                    ? (dynamic) textContent
                    : textContent.GetAs<TValue>();

                return value;
            });
    }
}