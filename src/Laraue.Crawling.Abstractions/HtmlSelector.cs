namespace Laraue.Crawling.Abstractions;

public sealed record HtmlSelector : Selector
{
    public HtmlSelector(string value)
        : base(value)
    {
    }

    public static implicit operator HtmlSelector(string value)
    {
        return new HtmlSelector(value);
    }
}