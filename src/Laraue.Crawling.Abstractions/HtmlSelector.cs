namespace Laraue.Crawling.Abstractions;

/// <summary>
/// Represent html selector.
/// </summary>
public record HtmlSelector
{
    /// <summary>
    /// Html selector.
    /// </summary>
    public string Selector { get; init; }

    /// <summary>
    /// Cast string to the <see cref="HtmlSelector"/>.
    /// </summary>
    /// <param name="selector"></param>
    /// <returns></returns>
    public static implicit operator HtmlSelector(string selector)
    {
        return new HtmlSelector { Selector = selector };
    }

    public override string ToString()
    {
        return Selector;
    }
};