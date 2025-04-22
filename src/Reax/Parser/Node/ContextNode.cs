using Reax.Parser.Node.Interfaces;
using Reax.Runtime;
using Reax.Semantic.Interfaces;
using Reax.Semantic.Scopes;
using Reax.Semantic.Symbols;

namespace Reax.Parser.Node;

public record ContextNode(
    ReaxNode[] Block, 
    SourceLocation Location) : ReaxNode(Location), IReaxContext, IReaxType, IReaxChildren
{
    public ReaxNode[] Context => Block;

    public ReaxNode[] Children => Block;

    public Symbol[] GetParameters(Guid scope)
        => [];

    public SymbolType GetReaxType(IReaxScope scope)
    {
        foreach (var node in Block)
        {
            if(node is IReaxType type)
                return type.GetReaxType(scope);
        }

        return SymbolType.NONE;
    }

    public override string ToString()
    {
        return "{...}";
    }
}
