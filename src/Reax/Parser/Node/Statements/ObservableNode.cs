using Reax.Parser.Node.Expressions;
using Reax.Parser.Node.Interfaces;

namespace Reax.Parser.Node.Statements;

public record ObservableNode(
    VarNode Var, 
    ContextNode Block, 
    BinaryNode? Condition, 
    SourceLocation Location) : ReaxNode(Location)
{
    public override string ToString()
    {
        var when = Condition is null ? "" : $"whe {Condition} "; 
        return $"on {Var} {when}{{...}}";
    }
}
