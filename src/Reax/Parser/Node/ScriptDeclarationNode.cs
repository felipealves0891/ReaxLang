using System;
using Reax.Semantic.Node;

namespace Reax.Parser.Node;

public record ScriptDeclarationNode(
    string Identifier, 
    SourceLocation Location) 
    : ReaxNode(Location), INode
{
    public bool IsLeaf => false;
    public INode[] Children => [];

    public override string ToString()
    {
        return $"script {Identifier};";
    }
}
