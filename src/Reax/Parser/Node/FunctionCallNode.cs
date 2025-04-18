namespace Reax.Parser.Node;

public record FunctionCallNode(string Identifier, ReaxNode[] Parameter, 
    SourceLocation Location) : ReaxNode(Location)
{
    public override string ToString()
    {
        return $"{Identifier}({string.Join(',', Parameter.Select(x => x.ToString()))});";
    }
}
