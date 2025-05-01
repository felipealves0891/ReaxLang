using System;

namespace Reax.Parser.Node;

public record ScriptDeclarationNode(
    string Identifier, 
    SourceLocation Location) 
    : ReaxNode(Location)
{
    public override string ToString()
    {
        return $"script {Identifier};";
    }
}
