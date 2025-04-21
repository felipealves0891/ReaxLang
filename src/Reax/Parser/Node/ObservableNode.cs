using Reax.Parser.Node.Interfaces;

namespace Reax.Parser.Node;

public record ObservableNode(
    ReaxNode Var, 
    ContextNode Context, 
    BinaryNode? Condition, 
    SourceLocation Location) : ReaxNode(Location), IReaxContext
{
    public ReaxNode[] Branchs => Context.Branchs;

    public override string ToString()
    {
        var when = Condition is null ? "" : $"whe {Condition.ToString()} "; 
        return $"on {Var} {when}{{...}}";
    }
}
