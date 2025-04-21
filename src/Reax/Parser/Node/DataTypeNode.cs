using System;

namespace Reax.Parser.Node;

public record DataTypeNode(string TypeName, SourceLocation Location) : ReaxNode(Location)
{
    public override string ToString()
    {
        return $": {TypeName}";
    }
}
