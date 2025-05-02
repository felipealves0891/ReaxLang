using Reax.Semantic;

namespace Reax.Parser.Node.Statements;

public record ForNode(
    DeclarationNode Declaration, 
    ReaxNode Condition, 
    ContextNode Block, 
    SourceLocation Location) : StatementNode(Location)
{
    public override IReaxNode[] Children => [Declaration, Condition, Block];

    public override string ToString()
    {
        return $"for {Declaration} to {Condition} {{...}}";
    }
}
