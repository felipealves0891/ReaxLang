using Reax.Parser.Node.Interfaces;
using Reax.Semantic.Interfaces;
using Reax.Semantic.Symbols;

namespace Reax.Parser.Node;

public record ObservableNode(
    VarNode Var, 
    ContextNode Block, 
    BinaryNode? Condition, 
    SourceLocation Location) : ReaxNode(Location), IReaxContext, IReaxChildren, IReaxBinder
{
    public ReaxNode[] Context => Block.Context.Concat(GetItems()).ToArray();

    public ReaxNode[] Children => Block.Context.Concat(GetItems()).ToArray();

    public string Identifier => Var.Identifier;

    public IReaxChildren Bound => (IReaxChildren)Block;

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
