using Laraue.Crawling.Abstractions;
using Laraue.Crawling.Common.Impl;
using PuppeteerSharp;

namespace Laraue.Crawling.Dynamic.PuppeterSharp;

/// <inheritdoc />
public class PuppeterSharpElementSchema<TModel> : ElementSchema<IElementHandle, HtmlSelector, TModel?>
{
    /// <inheritdoc />
    public PuppeterSharpElementSchema(Action<PropertyBuilder<IElementHandle, HtmlSelector, GenericCrawlingModel<TModel?>, TModel?>> propertyBuilder)
        : base(new PuppeterSharpSchemaBuilder<GenericCrawlingModel<TModel?>>(), propertyBuilder)
    {
    }
}