using Reax.Parser.Node.Expressions;
using Reax.Parser.Node.Interfaces;
using Reax.Semantic;

namespace Reax.Parser.Node.Statements;

public record ObservableNode(
    VarNode Var, 
    ContextNode Block, 
    BinaryNode? Condition, 
    SourceLocation Location) : StatementNode(Location)
{
    public override IReaxNode[] Children => Condition is null ? [Var, Block] : [Var, Block, Condition];
    public override string ToString()
    {
        var when = Condition is null ? "" : $"whe {Condition} "; 
        return $"on {Var} {when}{{...}}";
    }
}
