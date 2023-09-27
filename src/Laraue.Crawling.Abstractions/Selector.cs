namespace Laraue.Crawling.Abstractions;

/// <summary>
/// Represent html selector.
/// </summary>
public abstract record Selector(string Value)
{
    /// <summary>
    /// Selector string value.
    /// </summary>
    public string Value { get; init; } = Value;

    public override string ToString()
    {
        return Value;
    }
};