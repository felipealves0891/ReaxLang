namespace Reax.Parser.Node;

public record AssignmentNode(string Identifier, ReaxNode Assignment) : ReaxNode
{
    public override string ToString()
    {
        return $"{Identifier} = {Assignment};";
    }
}
