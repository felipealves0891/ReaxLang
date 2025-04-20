using System;

namespace Reax.Parser;

public struct SourceLocation
{
    public SourceLocation(string file, int line, int position)
    {
        File = file;
        Line = line;
        Position = position;
    }

    public string File { get;  }
    public int Line { get; }
    public int Position { get; }

    public override string ToString()
    {
        return $"{File}({Line})#{Position}";
    }
}
