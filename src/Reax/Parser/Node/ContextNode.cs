using Reax.Parser.Node.Interfaces;
using Reax.Runtime;
using Reax.Semantic.Interfaces;
using Reax.Semantic.Symbols;

namespace Reax.Parser.Node;

public record ContextNode(
    ReaxNode[] Block, 
    SourceLocation Location) : ReaxNode(Location), IReaxContext
{
    public ReaxNode[] Context => Block;

    public Symbol[] GetParameters(Guid scope)
        => [];

    public override string ToString()
    {
        return "{...}";
    }
}
