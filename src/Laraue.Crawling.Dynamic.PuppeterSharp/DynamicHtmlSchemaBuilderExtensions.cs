using System.Linq.Expressions;
using Laraue.Crawling.Abstractions;
using PuppeteerSharp;

namespace Laraue.Crawling.Dynamic.PuppeterSharp;

public static class DynamicHtmlSchemaBuilderExtensions
{
    public static IDynamicHtmlSchemaBuilder<TModel, Page, ElementHandle> ParseProperty<TModel>(
        this IDynamicHtmlSchemaBuilder<TModel, Page, ElementHandle> schema,
        Expression<Func<TModel, string>> schemaProperty,
        HtmlSelector htmlSelector)
    {
        return schema.ParseProperty(schemaProperty, htmlSelector, async element =>
        {
            if (element is null)
            {
                throw new Exception($"Handle {schemaProperty} error. The element is null.");
            }

            return await element.GetTrimmedInnerTextAsync();
        });
    }
    
    public static IDynamicHtmlSchemaBuilder<TModel, Page, ElementHandle> ParseProperty<TModel>(
        this IDynamicHtmlSchemaBuilder<TModel, Page, ElementHandle> schema,
        Expression<Func<TModel, int>> schemaProperty,
        HtmlSelector htmlSelector)
    {
        return schema.ParseProperty(schemaProperty, htmlSelector, element =>
        {
            if (element is null)
            {
                throw new Exception($"Handle {schemaProperty} error. The element is null.");
            }
            
            return element.GetIntAsync();
        });
    }
    
    public static IDynamicHtmlSchemaBuilder<TModel, Page, ElementHandle> ParseProperty<TModel>(
        this IDynamicHtmlSchemaBuilder<TModel, Page, ElementHandle> schema,
        Expression<Func<TModel, decimal>> schemaProperty,
        HtmlSelector htmlSelector)
    {
        return schema.ParseProperty(schemaProperty, htmlSelector, element => element.GetDecimalAsync());
    }
}