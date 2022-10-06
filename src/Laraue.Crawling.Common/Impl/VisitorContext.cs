using System.Runtime.CompilerServices;

namespace Laraue.Crawling.Common.Impl;

public class VisitorContext
{
    public VisitorContext()
    {
        CurrentPath = Array.Empty<Segment>();
    }

    public VisitorContext Push(int index)
    {
        return new VisitorContext
        {
            CurrentPath = CurrentPath.Concat(new[] {new ArraySegment(index)}).ToArray()
        };
    }
    
    public VisitorContext Push(string segment)
    {
        return new VisitorContext
        {
            CurrentPath = CurrentPath.Concat(new[] {new StringSegment(segment)}).ToArray()
        };
    }

    private Segment[] CurrentPath { get; init; }

    public override string ToString()
    {
        var sb = new DefaultInterpolatedStringHandler();

        for (var i = 0; i < CurrentPath.Length; i++)
        {
            var segment = CurrentPath[i];
            switch (segment)
            {
                case ArraySegment arraySegment:
                    sb.AppendFormatted('[');
                    sb.AppendFormatted(arraySegment.Index);
                    sb.AppendFormatted(']');
                    break;
                case StringSegment stringSegment:
                {
                    if (i != 0)
                    {
                        sb.AppendFormatted('.');
                    }
                    
                    sb.AppendFormatted(stringSegment.Value);
                    break;
                }
            }
        }

        return sb.ToStringAndClear();
    }
}

public record Segment;

public record ArraySegment(int Index) : Segment;

public record StringSegment(string Value) : Segment;