namespace Reax.Parser.Node.Expressions;

public record CalculateNode(
    ReaxNode Left, 
    ReaxNode Operator, 
    ReaxNode Right, 
    SourceLocation Location) : ExpressionNode(Location)
{
    public override string ToString()
    {
        return $"{Left} {Operator} {Right}";
    }
}
