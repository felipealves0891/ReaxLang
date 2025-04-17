namespace Reax.Parser.Node;

public record FunctionCallNode(string Identifier, ReaxNode[] Parameter) : ReaxNode
{
    public override string ToString()
    {
        return $"{Identifier}({string.Join(',', Parameter.Select(x => x.ToString()))});";
    }
}
