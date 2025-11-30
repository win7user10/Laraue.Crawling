using System.Text;

namespace Laraue.Crawling.Common.Impl;

public class VisitorContext
{
    public VisitorContext()
    {
        CurrentPath = [];
    }

    public VisitorContext Push(int index)
    {
        return new VisitorContext
        {
            CurrentPath = CurrentPath.Concat([new ArraySegment(index)]).ToArray()
        };
    }
    
    public VisitorContext Push(string segment)
    {
        return new VisitorContext
        {
            CurrentPath = CurrentPath.Concat([new StringSegment(segment)]).ToArray()
        };
    }

    private Segment[] CurrentPath { get; init; }

    public override string ToString()
    {
        var sb = new StringBuilder();

        for (var i = 0; i < CurrentPath.Length; i++)
        {
            var segment = CurrentPath[i];
            switch (segment)
            {
                case ArraySegment arraySegment:
                    sb.Append('[');
                    sb.Append(arraySegment.Index);
                    sb.Append(']');
                    break;
                case StringSegment stringSegment:
                {
                    if (i != 0)
                    {
                        sb.Append('.');
                    }
                    
                    sb.Append(stringSegment.Value);
                    break;
                }
            }
        }

        return sb.ToString();
    }
}

public record Segment;

public record ArraySegment(int Index) : Segment;

public record StringSegment(string Value) : Segment;