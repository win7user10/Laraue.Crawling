using AngleSharp.Dom;
using Laraue.Crawling.Abstractions;
using Laraue.Crawling.Common.Impl;

namespace Laraue.Crawling.Static.AngleSharp;

/// <inheritdoc />
public class AngleSharpSchemaBuilder<TModel> : DocumentSchemaBuilder<IElement, HtmlSelector, TModel>
    where TModel : class, ICrawlingModel
{
    /// <inheritdoc />
    public AngleSharpSchemaBuilder()
        : base(new AngleSharpPropertyBuilderFactory())
    {
    }
}