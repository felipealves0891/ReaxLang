using System;

namespace Reax.Core.Locations;

public struct SourceLocation
{
    public SourceLocation(string file, Position start, Position end)
    {
        File = file;
        Start = start;
        End = end;
    }

    public string File { get;  }
    public Position Start { get; }
    public Position End { get; }

    public override string ToString()
    {
        return $"{File}({Start.Line}:{Start.Column}, {End.Line}:{End.Column})";
    }
}
