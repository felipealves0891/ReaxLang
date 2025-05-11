using System.Diagnostics.CodeAnalysis;
using Reax.Core.Locations;
using Reax.Parser.Node.Interfaces;
using Reax.Semantic;

namespace Reax.Parser.Node.Statements;

[ExcludeFromCodeCoverage]
public record AssignmentNode(
    VarNode Identifier, 
    ReaxNode Assigned, 
    SourceLocation Location) : StatementNode(Location)
{
    public override IReaxNode[] Children => [Assigned];

    public override string ToString()
    {
        return $"{Identifier} = {Assigned};";
    }
}
