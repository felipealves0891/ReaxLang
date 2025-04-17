namespace Reax.Parser.Node;

public record ReturnNode(ReaxNode Expression) : ReaxNode
{
    public override string ToString()
    {
        return $"return {Expression}";
    }
}
