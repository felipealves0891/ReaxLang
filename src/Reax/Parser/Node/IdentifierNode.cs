using System;
using Reax.Parser.Node.Interfaces;

namespace Reax.Parser.Node;

public record IdentifierNode(
    string Identifier, 
    SourceLocation Location) : ReaxNode(Location)
{
    public override string ToString()
    {
        return Identifier;
    }
}
