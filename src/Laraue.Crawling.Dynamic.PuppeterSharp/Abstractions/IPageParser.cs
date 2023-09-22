using Laraue.Crawling.Abstractions;
using PuppeteerSharp;

namespace Laraue.Crawling.Dynamic.PuppeterSharp.Abstractions;

/// <summary>
/// Abstraction that transforms opened browser page to the crawling model.
/// </summary>
public interface IPageParser
{
    /// <summary>
    /// Get the opened page and schema and returns the crawling result.
    /// </summary>
    /// <param name="page"></param>
    /// <param name="schema"></param>
    /// <returns></returns>
    Task<TResult> ParseAsync<TResult>(IPage page, ICompiledHtmlSchema<IElementHandle, TResult> schema);
}