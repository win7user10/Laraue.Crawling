using System.Linq.Expressions;
using Laraue.Crawling.Static.Abstractions;

namespace Laraue.Crawling.Static.Impl;

public static class HtmlSchemaBuilderExtensions
{
    public static IHtmlSchemaBuilder<TModel> HasProperty<TModel>(
        this IHtmlSchemaBuilder<TModel> htmlSchemaBuilder,
        Expression<Func<TModel, string>> schemaProperty,
        HtmlSelector htmlSelector)
    {
        return htmlSchemaBuilder.HasProperty(schemaProperty, htmlSelector, s => s?.GetInnerHtml());
    }
    
    public static IHtmlSchemaBuilder<TModel> HasArrayProperty<TModel>(
        this IHtmlSchemaBuilder<TModel> htmlSchemaBuilder,
        Expression<Func<TModel, string[]>> schemaProperty,
        HtmlSelector htmlSelector)
    {
        return htmlSchemaBuilder.HasArrayProperty(schemaProperty, htmlSelector, s => s?.GetInnerHtml());
    }
}