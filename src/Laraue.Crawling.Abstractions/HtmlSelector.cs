namespace Laraue.Crawling.Abstractions;

public sealed record HtmlSelector : Selector
{
    /// <inheritdoc />
    public HtmlSelector(string value)
        : base(value)
    {
    }

    /// <summary>
    /// Returns new <see cref="HtmlSelector"/> parsed from the string.
    /// </summary>
    public static implicit operator HtmlSelector(string value)
    {
        return new HtmlSelector(value);
    }
}