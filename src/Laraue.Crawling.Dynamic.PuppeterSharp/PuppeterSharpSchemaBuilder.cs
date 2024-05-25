using Laraue.Crawling.Abstractions;
using Laraue.Crawling.Common.Impl;
using PuppeteerSharp;

namespace Laraue.Crawling.Dynamic.PuppeterSharp;

/// <summary>
/// Builder of the <see cref="CompiledDocumentSchema{TElement,HtmlSelector,TModel}"/> for the puppeter.
/// </summary>
/// <typeparam name="TModel"></typeparam>
public class PuppeterSharpSchemaBuilder<TModel> : DocumentSchemaBuilder<IElementHandle, HtmlSelector, TModel>
    where TModel : class, ICrawlingModel
{
    public PuppeterSharpSchemaBuilder()
        : base(new PuppeterPropertyBuilderFactory())
    {
    }
}