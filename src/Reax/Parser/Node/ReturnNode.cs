namespace Reax.Parser.Node;

public record ReturnNode(ReaxNode Expression, 
    SourceLocation Location) : ReaxNode(Location)
{
    public override string ToString()
    {
        return $"return {Expression}";
    }
}
