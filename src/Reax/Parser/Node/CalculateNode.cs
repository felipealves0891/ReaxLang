namespace Reax.Parser.Node;

public record CalculateNode(ReaxNode Left, ReaxNode Operator, ReaxNode Right) : ReaxNode
{
    public override string ToString()
    {
        return $"{Left} {Operator} {Right}";
    }
}
