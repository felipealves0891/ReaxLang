using Reax.Parser.Node.Interfaces;

namespace Reax.Parser.Node.Statements;

public record AssignmentNode(
    VarNode Identifier, 
    ReaxNode Assigned, 
    SourceLocation Location) : StatementNode(Location)
{
    public override string ToString()
    {
        return $"{Identifier} = {Assigned};";
    }
}
