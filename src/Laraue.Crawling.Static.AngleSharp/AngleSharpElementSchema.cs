using AngleSharp.Dom;
using Laraue.Crawling.Abstractions;
using Laraue.Crawling.Common.Impl;

namespace Laraue.Crawling.Static.AngleSharp;

/// <inheritdoc />
public class AngleSharpElementSchema<TModel> : ElementSchema<IElement, HtmlSelector, TModel?>
{
    /// <inheritdoc />
    public AngleSharpElementSchema(Action<PropertyBuilder<IElement, HtmlSelector, GenericResponse<TModel?>, TModel?>> propertyBuilder)
        : base(new AngleSharpSchemaBuilder<GenericResponse<TModel?>>(), propertyBuilder)
    {
    }
}