namespace Reax.Parser.Node;

public record FunctionNode(ReaxNode Identifier, ReaxNode Block, ReaxNode[] Parameters) : ReaxNode
{
    public override string ToString()
    {
        var param = string.Join(',', Parameters.Select(x => x.ToString()));
        return $"fun {Identifier} ({param}) {{...}}";
    }
}
