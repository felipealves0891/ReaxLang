using Reax.Parser.Node.Interfaces;
using Reax.Semantic.Interfaces;
using Reax.Semantic.Symbols;

namespace Reax.Parser.Node;

public record ObservableNode(
    VarNode Var, 
    ContextNode Block, 
    BinaryNode? Condition, 
    SourceLocation Location) : ReaxNode(Location), IReaxContext
{
    public ReaxNode[] Context => Block.Context.Concat(GetItems()).ToArray();

    public Symbol[] GetParameters(Guid scope)
        => [];

    private ReaxNode[] GetItems() 
    {
        if(Condition is null)
            return [Var];
        else 
            return [Var, Condition];
    }

    public override string ToString()
    {
        var when = Condition is null ? "" : $"whe {Condition} "; 
        return $"on {Var} {when}{{...}}";
    }
}
