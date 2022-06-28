using System.Text.RegularExpressions;
using PuppeteerSharp;

namespace Laraue.Crawling.Dynamic.PuppeterSharp;

public static class ElementHandleExtensions
{
    public static readonly Regex NonDigitCharsRegex = new(@"[^\d]", RegexOptions.Compiled);
    public static readonly Regex FloatCharsRegex = new(@"\d+(\.\d+)?", RegexOptions.Compiled);
    
    public static Task<string> GetInnerHtmlAsync(this ElementHandle elementHandle)
    {
        return elementHandle.EvaluateFunctionAsync<string>("e => e.innerText");
    }
    
    public static Task<string> GetClassNameAsync(this ElementHandle elementHandle)
    {
        return elementHandle.EvaluateFunctionAsync<string>("e => e.className");
    }
    
    public static Task<string> GetTrimmedInnerHtmlAsync(this ElementHandle elementHandle)
    {
        return GetInnerHtmlAsync(elementHandle).AwaitAndModify(s => s.Trim());
    }
    
    public static Task<int> GetIntAsync(this ElementHandle elementHandle)
    {
        return GetInnerHtmlAsync(elementHandle).AwaitAndModify(GetIntAsync);
    }
    
    public static Task<decimal> GetDecimalAsync(this ElementHandle elementHandle)
    {
        return GetInnerHtmlAsync(elementHandle).AwaitAndModify(GetDecimalAsync);
    }
    
    public static int GetIntAsync(this string str)
    {
        return int.Parse(NonDigitCharsRegex.Replace(str, string.Empty));
    }
    
    public static decimal GetDecimalAsync(this string str)
    {
        return decimal.Parse(FloatCharsRegex.Match(str).Value);
    }
}