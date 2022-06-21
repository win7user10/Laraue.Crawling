namespace Laraue.Crawling.Static.Abstractions;

public interface IHtmlElement
{
    string GetInnerHtml();
    string? GetAttribute(string name);
}