using System;
using Reax.Parser.Node.Interfaces;
using Reax.Semantic.Nodes;

namespace Reax.Parser.Node;

public record IdentifierNode(
    string Identifier, 
    SourceLocation Location) : ReaxNode(Location), INode
{
    public bool IsLeaf => true;
    public INode[] Children => [];

    public override string ToString()
    {
        return Identifier;
    }
}
