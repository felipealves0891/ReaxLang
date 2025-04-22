using Reax.Parser.Node.Interfaces;
using Reax.Runtime;
using Reax.Semantic.Interfaces;
using Reax.Semantic.Symbols;

namespace Reax.Parser.Node;

public record ReturnNode(
    ReaxNode Expression, 
    SourceLocation Location) : ReaxNode(Location), IReaxContext, IReaxChildren
{
    public ReaxNode[] Context => Expression is ContextNode block ? block.Context : [Expression];

    public ReaxNode[] Children => Expression is ContextNode block ? block.Context : [Expression];

    public Symbol[] GetParameters(Guid scope)
        => [];

    public override string ToString()
    {
        return $"return {Expression}";
    }
}
