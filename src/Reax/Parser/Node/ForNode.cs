namespace Reax.Parser.Node;

public record ForNode(ReaxNode declaration, ReaxNode condition, ReaxNode Block) : ReaxNode 
{
    public override string ToString()
    {
        return $"for {declaration} to {condition} {{}}";
    }
}
