using PuppeteerSharp;

namespace Laraue.Crawling.Dynamic.PuppeterSharp;

public static class ElementHandleExtensions
{
    public static Task<string> GetInnerHtmlAsync(this ElementHandle elementHandle)
    {
        return elementHandle.EvaluateFunctionAsync<string>("e => e.innerText");
    }
    
    public static Task<string> GetTrimmedInnerHtmlAsync(this ElementHandle elementHandle)
    {
        return GetInnerHtmlAsync(elementHandle).AwaitAndModify(s => s.Trim());
    }
}