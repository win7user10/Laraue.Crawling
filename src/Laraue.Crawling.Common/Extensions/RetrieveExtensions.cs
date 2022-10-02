using System.Text.Json;
using System.Text.RegularExpressions;

namespace Laraue.Crawling.Common.Extensions;

public static class RetrieveExtensions
{
    private static readonly Regex NonDigitCharsRegex = new(@"[^\d]", RegexOptions.Compiled);
    private static readonly Regex FloatCharsRegex = new(@"\d+(\.\d+)?", RegexOptions.Compiled);
    
    /// <summary>
    /// Get only integers from string and parse them.
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static int GetIntOrDefault(this string? str)
    {
        if (string.IsNullOrEmpty(str))
        {
            return default;
        }
        
        var intString = NonDigitCharsRegex.Replace(str, string.Empty);
        return intString.GetAs<int>();
    }
    
    /// <summary>
    /// Get only decimal from string and parse them.
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static decimal GetDecimalOrDefault(this string? str)
    {
        if (string.IsNullOrEmpty(str))
        {
            return default;
        }
        
        var decString = FloatCharsRegex.Match(str).Value;
        return decString.GetAs<decimal>();
    }
    
    private static T? GetAs<T>(this string str)
    {
        return JsonSerializer.Deserialize<T>(str);
    }
}