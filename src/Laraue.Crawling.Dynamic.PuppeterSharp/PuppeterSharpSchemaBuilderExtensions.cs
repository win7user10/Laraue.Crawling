using System.Linq.Expressions;
using Laraue.Crawling.Abstractions;
using Laraue.Crawling.Common.Extensions;
using Laraue.Crawling.Common.Impl;
using PuppeteerSharp;

namespace Laraue.Crawling.Dynamic.PuppeterSharp;

/// <summary>
/// Often used methods while writing puppeteer schema.
/// </summary>
public static class PuppeterSharpSchemaBuilderExtensions
{
    /// <summary>
    /// Binds the selected property with an attribute value of the
    /// selected element casted to the specified type.
    /// </summary>
    /// <returns></returns>
    public static HtmlSchemaBuilder<IElementHandle, TModel> HasProperty<TModel, TValue>(
        this HtmlSchemaBuilder<IElementHandle, TModel> schemaBuilder,
        Expression<Func<TModel, TValue?>> schemaProperty,
        HtmlSelector htmlSelector,
        string attributeName)
    {
        return schemaBuilder.HasProperty(
            schemaProperty,
            htmlSelector,
            element => element.GetAttributeValueAsync(attributeName));
    }
    
    /// <summary>
    /// Binds the selected property with an attribute value of the
    /// selected element mapped to the specified type by the passed function.
    /// </summary>
    /// <returns></returns>
    public static HtmlSchemaBuilder<IElementHandle, TModel> HasProperty<TModel, TValue>(
        this HtmlSchemaBuilder<IElementHandle, TModel> schemaBuilder,
        Expression<Func<TModel, TValue?>> schemaProperty,
        HtmlSelector htmlSelector,
        Func<string?, TValue?> getValue,
        string attributeName)
    {
        return schemaBuilder.HasProperty(
            schemaProperty,
            htmlSelector,
            async element =>
            {
                var value = await element.GetAttributeValueAsync(attributeName).ConfigureAwait(false);
                return getValue(value);
            });
    }

    /// <summary>
    /// Binds the selected property with a value taken from the
    /// inner text mapped by the passed function.
    /// </summary>
    /// <returns></returns>
    public static HtmlSchemaBuilder<IElementHandle, TModel> HasProperty<TModel, TValue>(
        this HtmlSchemaBuilder<IElementHandle, TModel> schemaBuilder,
        Expression<Func<TModel, TValue?>> schemaProperty,
        HtmlSelector htmlSelector,
        Func<string?, TValue?> getValue)
    {
        return schemaBuilder.HasProperty(
            schemaProperty,
            htmlSelector,
            async element =>
            {
                var text = await element.GetInnerTextAsync().ConfigureAwait(false);
                return getValue(text);
            });
    }

    /// <summary>
    /// Binds the selected property with an inner text of the
    /// selected element casted to the specified type.
    /// </summary>
    /// <returns></returns>
    public static HtmlSchemaBuilder<IElementHandle, TModel> HasProperty<TModel, TValue>(
        this HtmlSchemaBuilder<IElementHandle, TModel> schemaBuilder,
        Expression<Func<TModel, TValue?>> schemaProperty,
        HtmlSelector htmlSelector)
    {
        return schemaBuilder.HasProperty(
            schemaProperty,
            htmlSelector,
            element => element.GetInnerTextAsync());
    }
    
    /// <summary>
    /// Binds the selected property with a value taken from the
    /// passed async delegate and casted to the specified type.
    /// </summary>
    /// <returns></returns>
    public static HtmlSchemaBuilder<IElementHandle, TModel> HasProperty<TModel, TValue>(
        this HtmlSchemaBuilder<IElementHandle, TModel> schemaBuilder,
        Expression<Func<TModel, TValue?>> schemaProperty,
        HtmlSelector htmlSelector,
        Func<IElementHandle, Task<string?>> getValueTask)
    {
        return schemaBuilder.HasProperty(
            schemaProperty,
            htmlSelector,
            async element =>
            {
                var htmlString = await getValueTask(element).ConfigureAwait(false);
                if (htmlString is null)
                {
                    return default;
                }
                
                return typeof(TValue) == typeof(string)
                    ? (TValue)(dynamic) htmlString
                    : htmlString.GetAs<TValue>();
            });
    }
}