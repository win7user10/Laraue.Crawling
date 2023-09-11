using Laraue.Crawling.Common.Impl;
using PuppeteerSharp;

namespace Laraue.Crawling.Dynamic.PuppeterSharp;

/// <summary>
/// Builder of the <see cref="CompiledHtmlSchema{TElement,TModel}"/> for the puppeter.
/// </summary>
/// <typeparam name="TModel"></typeparam>
public class PuppeterSharpSchemaBuilder<TModel> : HtmlSchemaBuilder<IElementHandle, TModel>
{
}