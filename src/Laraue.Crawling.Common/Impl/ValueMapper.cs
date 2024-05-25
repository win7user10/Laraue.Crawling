using System.Text.Json;

namespace Laraue.Crawling.Common.Impl;

public abstract class ValueMapper
{
    public abstract TValue? Map<TValue>(string? value);
    public abstract object? Map(string? value, Type type);
}

public sealed class JsonValueMapper : ValueMapper
{
    public override TValue? Map<TValue>(string? value) where TValue : default
    {
        return (TValue?) Map(value, typeof(TValue));
    }

    public override object? Map(string? value, Type type)
    {
        if (value is null)
        {
            return default;
        }
        
        return type == typeof(string)
            ? value
            : JsonSerializer.Deserialize(value, type);
    }
}