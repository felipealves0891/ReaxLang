namespace Reax.Parser.Node;

public record ContextNode(ReaxNode[] Block) : ReaxNode
{
    public override string ToString()
    {
        return "{...}";
    }
}
