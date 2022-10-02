using System.Linq.Expressions;
using Laraue.Crawling.Abstractions;
using Laraue.Crawling.Common.Impl;
using PuppeteerSharp;

namespace Laraue.Crawling.Dynamic.PuppeterSharp;

public static class PuppeterSharpSchemaBuilderExtensions
{
    public static HtmlSchemaBuilder<ElementHandle, TModel> HasProperty<TModel>(
        this HtmlSchemaBuilder<ElementHandle, TModel> schema,
        Expression<Func<TModel, string>> schemaProperty,
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
    
    public static HtmlSchemaBuilder<ElementHandle, TModel> HasProperty<TModel>(
        this HtmlSchemaBuilder<ElementHandle, TModel> schema,
        Expression<Func<TModel, int>> schemaProperty,
        HtmlSelector htmlSelector)
    {
        return schema.HasProperty(schemaProperty, htmlSelector, element =>
        {
            if (element is null)
            {
                throw new Exception($"Handle {schemaProperty} error. The element is null.");
            }
            
            return element.GetIntAsync();
        });
    }
    
    public static HtmlSchemaBuilder<ElementHandle, TModel> HasProperty<TModel>(
        this HtmlSchemaBuilder<ElementHandle, TModel> schema,
        Expression<Func<TModel, decimal>> schemaProperty,
        HtmlSelector htmlSelector)
    {
        return schema.HasProperty(schemaProperty, htmlSelector, element => element.GetDecimalAsync());
    }
}