using Reax.Semantic;

namespace Reax.Parser.Node.Statements;

public record WhileNode(
    ReaxNode Condition, 
    ContextNode Block, 
    SourceLocation Location) : StatementNode(Location)
{
    public override IReaxNode[] Children => [Condition, Block];

    public override string ToString()
    {
        return $"while {Condition} {{...}}";
    }
}
