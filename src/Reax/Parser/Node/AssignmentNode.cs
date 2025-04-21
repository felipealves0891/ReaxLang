using Reax.Parser.Node.Interfaces;

namespace Reax.Parser.Node;

public record AssignmentNode(
    string Identifier, 
    ReaxNode Assigned, 
    SourceLocation Location) : ReaxNode(Location)
{
    public override string ToString()
    {
        return $"{Identifier} = {Assigned};";
    }
}
