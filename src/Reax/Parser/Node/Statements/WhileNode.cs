namespace Reax.Parser.Node.Statements;

public record WhileNode(
    ReaxNode condition, 
    ContextNode Block, 
    SourceLocation Location) : StatementNode(Location)
{
    public override string ToString()
    {
        return $"while {condition} {{...}}";
    }
}
