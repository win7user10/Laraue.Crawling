using AngleSharp.Dom;
using Laraue.Crawling.Common.Impl;

namespace Laraue.Crawling.Static.AngleSharp;

/// <inheritdoc />
public class AngleSharpPropertyBuilderFactory : PropertyBuilderFactory<IElement>
{
    /// <inheritdoc />
    public AngleSharpPropertyBuilderFactory()
        : base(new AngleSharpCrawlingAdapter(new JsonValueMapper()))
    {
    }
}

/// <inheritdoc />
public class AngleSharpCrawlingAdapter : BaseCrawlingAdapter<IElement>
{
    /// <inheritdoc />
    public AngleSharpCrawlingAdapter(ValueMapper valueMapper) : base(valueMapper)
    {
    }

    /// <inheritdoc />
    public override Task<string?> GetInnerTextAsync(IElement? element)
    {
        return Task.FromResult(element?.TextContent);
    }

    /// <inheritdoc />
    public override Task<string?> GetAttributeTextAsync(IElement? element, string attributeName)
    {
        return Task.FromResult(element?.GetAttribute(attributeName));
    }
}