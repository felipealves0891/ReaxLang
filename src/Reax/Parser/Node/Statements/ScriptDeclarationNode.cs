using System;
using System.Diagnostics.CodeAnalysis;
using Reax.Core.Locations;
using Reax.Semantic;

namespace Reax.Parser.Node;

[ExcludeFromCodeCoverage]
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
