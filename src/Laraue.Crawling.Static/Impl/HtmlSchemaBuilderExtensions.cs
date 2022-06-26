using System.Linq.Expressions;
using Laraue.Crawling.Abstractions;
using Laraue.Crawling.Static.Abstractions;

namespace Laraue.Crawling.Static.Impl;

public static class HtmlSchemaBuilderExtensions
{
    public static IStaticHtmlSchemaBuilder<TModel> HasProperty<TModel>(
        this IStaticHtmlSchemaBuilder<TModel> staticHtmlSchemaBuilder,
        Expression<Func<TModel, string>> schemaProperty,
        HtmlSelector htmlSelector)
    {
        return staticHtmlSchemaBuilder.HasProperty(schemaProperty, htmlSelector, s => s?.GetInnerHtml());
    }
    
    public static IStaticHtmlSchemaBuilder<TModel> HasArrayProperty<TModel>(
        this IStaticHtmlSchemaBuilder<TModel> staticHtmlSchemaBuilder,
        Expression<Func<TModel, string[]>> schemaProperty,
        HtmlSelector htmlSelector)
    {
        return staticHtmlSchemaBuilder.HasArrayProperty(schemaProperty, htmlSelector, s => s?.GetInnerHtml());
    }
}