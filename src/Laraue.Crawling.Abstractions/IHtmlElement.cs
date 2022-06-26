namespace Laraue.Crawling.Abstractions;

/// <summary>
/// Represents html element in the static scheme.
/// </summary>
public interface IHtmlElement
{
    /// <summary>
    /// Returns inner html.
    /// </summary>
    /// <returns></returns>
    string GetInnerHtml();
    
    /// <summary>
    /// Returns content of attribute.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    string? GetAttribute(string name);
}