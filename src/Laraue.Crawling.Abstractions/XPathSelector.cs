namespace Laraue.Crawling.Abstractions;

public record XPathSelector : Selector
{
    public XPathSelector(string value)
        : base(value)
    {
    }

    public static implicit operator XPathSelector(string value)
    {
        return new XPathSelector(value);
    }
}