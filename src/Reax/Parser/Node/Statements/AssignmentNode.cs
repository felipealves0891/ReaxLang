using Reax.Parser.Node.Interfaces;
using Reax.Semantic;

namespace Reax.Parser.Node.Statements;

public record AssignmentNode(
    VarNode Identifier, 
    ReaxNode Assigned, 
    SourceLocation Location) : StatementNode(Location)
{
    public override IReaxNode[] Children => [];

    public override string ToString()
    {
        return $"{Identifier} = {Assigned};";
    }
}
