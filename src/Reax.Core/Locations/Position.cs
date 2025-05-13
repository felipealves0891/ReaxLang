using System;

namespace Reax.Core.Locations;

public struct Position
{
    public Position(int line, int column)
    {
        Line = line;
        Column = column;
    }

    public int Line { get; init; }
    public int Column { get; init; }
}
