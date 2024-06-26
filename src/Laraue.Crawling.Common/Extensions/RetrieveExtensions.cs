﻿using System.Text.RegularExpressions;

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
        return str.GetNumberOrDefault<int>(GetOnlyDigits);
    }
    
    /// <summary>
    /// Get only integers from string and parse them.
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static long GetLongOrDefault(this string? str)
    {
        return str.GetNumberOrDefault<long>(GetOnlyDigits);
    }

    /// <summary>
    /// Get only digits from the string.
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string GetOnlyDigits(this string? str)
    {
        return str is not null
            ? NonDigitCharsRegex.Replace(str, string.Empty)
            : string.Empty;
    }
    
    /// <summary>
    /// Get only decimal from string and parse them.
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static decimal GetDecimalOrDefault(this string? str)
    {
        return str.GetNumberOrDefault<decimal>(s => FloatCharsRegex.Match(s).Value);
    }

    private static T GetNumberOrDefault<T>(this string? str, Func<string, string> cleanString) where T : struct
    {
        if (string.IsNullOrEmpty(str))
        {
            return default;
        }

        var numberString = cleanString(str);

        return string.IsNullOrEmpty(numberString)
            ? default
            : StringValueMapper.Map<T>(numberString);
    }
}