namespace Reax.Parser.Node;

public record WhileNode(ReaxNode condition, ReaxNode Block) : ReaxNode
{
    public override string ToString()
    {
        return $"while {condition} {{...}}";
    }
}
