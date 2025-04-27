using System;

namespace Reax.Parser.Node;

public record NullNode(SourceLocation Location) : ReaxNode(Location)
{
    public override string ToString()
    {
        return "NULL";
    }
}
