namespace Reax.Parser.Node;

public record ObservableNode(ReaxNode Var, ReaxNode Block, BinaryNode? Condition) : ReaxNode
{
    public override string ToString()
    {
        return $"on {Var} {{...}}";
    }
}
