using Reax.Parser.Node.Expressions;
using Reax.Semantic;

namespace Reax.Parser.Node.Statements;

public record IfNode(
    BinaryNode Condition, 
    ContextNode True, 
    ContextNode? False, 
    SourceLocation Location) : StatementNode(Location)
{
    public override IReaxNode[] Children => False is null ? [Condition, True] : [Condition, True, False];

    public override string ToString()
    {
        var elseText = False is null ? "" : "else {}";
        return $"if {Condition} {{}} {elseText}";
    }
}
