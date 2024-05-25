using AngleSharp.Dom;
using Laraue.Crawling.Common.Impl;

namespace Laraue.Crawling.Static.AngleSharp;

public class AngleSharpPropertyBuilderFactory : PropertyBuilderFactory<IElement>
{
    public AngleSharpPropertyBuilderFactory()
        : base(new AngleSharpExtractors(new JsonValueMapper()))
    {
    }
}

public class AngleSharpExtractors : BaseExtractors<IElement>
{
    public AngleSharpExtractors(ValueMapper valueMapper) : base(valueMapper)
    {
    }

    public override Task<string?> GetInnerTextAsync(IElement element)
    {
        return Task.FromResult((string?) element.TextContent);
    }

    public override Task<string?> GetAttributeTextAsync(IElement element, string attributeName)
    {
        return Task.FromResult(element.GetAttribute(attributeName));
    }
}