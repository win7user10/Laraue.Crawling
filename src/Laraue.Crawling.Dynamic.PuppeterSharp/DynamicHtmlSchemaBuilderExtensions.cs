﻿using System.Linq.Expressions;
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
        return schema.ParseProperty(schemaProperty, htmlSelector, element => element.GetTrimmedInnerHtmlAsync());
    }
}