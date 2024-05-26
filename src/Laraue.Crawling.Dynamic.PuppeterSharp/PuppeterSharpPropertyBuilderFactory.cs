using Laraue.Crawling.Common.Impl;
using PuppeteerSharp;

namespace Laraue.Crawling.Dynamic.PuppeterSharp;

/// <inheritdoc />
public class PuppeterSharpPropertyBuilderFactory : PropertyBuilderFactory<IElementHandle>
{
    /// <inheritdoc />
    public PuppeterSharpPropertyBuilderFactory()
        : base(new PuppeterSharpCrawlingAdapter(new JsonValueMapper()))
    {
    }
}

/// <inheritdoc />
public class PuppeterSharpCrawlingAdapter : BaseCrawlingAdapter<IElementHandle>
{
    /// <inheritdoc />
    public PuppeterSharpCrawlingAdapter(ValueMapper valueMapper)
        : base(valueMapper)
    {
    }

    /// <inheritdoc />
    public override Task<string?> GetInnerTextAsync(IElementHandle? element)
    {
        return element.GetInnerTextAsync();
    }

    /// <inheritdoc />
    public override Task<string?> GetAttributeTextAsync(IElementHandle? element, string attributeName)
    {
        return element.GetAttributeValueAsync(attributeName);
    }
}