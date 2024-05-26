using Laraue.Crawling.Common.Impl;
using PuppeteerSharp;

namespace Laraue.Crawling.Dynamic.PuppeterSharp;

public class PuppeterPropertyBuilderFactory : PropertyBuilderFactory<IElementHandle>
{
    public PuppeterPropertyBuilderFactory()
        : base(new PuppeterSharpCrawlingAdapter(new JsonValueMapper()))
    {
    }
}

public class PuppeterSharpCrawlingAdapter : BaseCrawlingAdapter<IElementHandle>
{
    public PuppeterSharpCrawlingAdapter(ValueMapper valueMapper)
        : base(valueMapper)
    {
    }

    public override Task<string?> GetInnerTextAsync(IElementHandle element)
    {
        return element.GetInnerTextAsync();
    }

    public override Task<string?> GetAttributeTextAsync(IElementHandle element, string attributeName)
    {
        return element.GetAttributeValueAsync(attributeName);
    }
}