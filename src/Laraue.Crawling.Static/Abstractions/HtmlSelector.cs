namespace Laraue.Crawling.Static.Abstractions;

public record HtmlSelector
{
    public string Selector { get; init; }

    public static implicit operator HtmlSelector(string selector)
    {
        return new HtmlSelector { Selector = selector };
    }
};