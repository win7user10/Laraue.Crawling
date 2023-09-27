using System.Text.Json;

namespace Laraue.Crawling.Common;

public static class StringValueMapper
{
    public static TValue? Map<TValue>(string value)
    {
        return typeof(TValue) == typeof(string)
            ? (dynamic) value
            : JsonSerializer.Deserialize<TValue>(value);
    }
}