namespace Reax.Parser.Node;

public record FunctionNode(ReaxNode Identifier, ReaxNode Block, ReaxNode[] Parameters, 
    SourceLocation Location) : ReaxNode(Location)
{
    public override string ToString()
    {
        var param = string.Join(',', Parameters.Select(x => x.ToString()));
        return $"fun {Identifier} ({param}) {{...}}";
    }
}
