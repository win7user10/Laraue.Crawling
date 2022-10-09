using System.Linq.Expressions;
using System.Text.Json;
using Laraue.Crawling.Abstractions;
using Laraue.Crawling.Common.Impl;
using PuppeteerSharp;

namespace Laraue.Crawling.Dynamic.PuppeterSharp;

public static class PuppeterSharpSchemaBuilderExtensions
{
    public static HtmlSchemaBuilder<ElementHandle, TModel> HasProperty<TModel>(
        this HtmlSchemaBuilder<ElementHandle, TModel> schema,
        Expression<Func<TModel, string?>> schemaProperty,
        HtmlSelector htmlSelector)
    {
        return schema.HasProperty(schemaProperty, htmlSelector, async element =>
        {
            if (element is null)
            {
                throw new Exception($"Handle {schemaProperty} error. The element is null.");
            }

            return await element.GetTrimmedInnerTextAsync();
        });
    }
    
    public static HtmlSchemaBuilder<ElementHandle, TModel> HasProperty<TModel, TValue>(
        this HtmlSchemaBuilder<ElementHandle, TModel> schemaBuilder,
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

                if (modifyFunc is not null)
                {
                    textContent = modifyFunc(textContent);
                }
                
                var value = typeof(TValue) == typeof(string)
                    ? (dynamic) textContent
                    : JsonSerializer.Deserialize<TValue>(textContent);

                return value;
            });
    }
}