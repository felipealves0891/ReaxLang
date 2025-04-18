namespace Reax.Parser.Node;

public record BinaryNode(
    ReaxNode Left, 
    ReaxNode Operator, 
    ReaxNode Right, 
    SourceLocation Location) : ReaxNode(Location)
{
    public override string ToString()
    {
        return $"{Left} {Operator} {Right}";
    }
}
