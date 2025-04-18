namespace Reax.Parser.Node;

public record AssignmentNode(
    string Identifier, 
    ReaxNode Assignment, 
    SourceLocation Location) : ReaxNode(Location)
{
    public override string ToString()
    {
        return $"{Identifier} = {Assignment};";
    }
}
