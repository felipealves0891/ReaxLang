using System;
using Reax.Semantic;

namespace Reax.Parser.Node;

public record ScriptDeclarationNode(
    string Identifier, 
    SourceLocation Location) 
    : ReaxNode(Location)
{
    public override IReaxNode[] Children => [];

    public override string ToString()
    {
        return $"script {Identifier};";
    }
}
