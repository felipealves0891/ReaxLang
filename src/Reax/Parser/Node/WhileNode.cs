using Reax.Parser.Helper;
using Reax.Parser.Node.Interfaces;
using Reax.Semantic.Interfaces;
using Reax.Semantic.Symbols;

namespace Reax.Parser.Node;

public record WhileNode(
    ReaxNode condition, 
    ContextNode Block, 
    SourceLocation Location) : ReaxNode(Location), IReaxContext, IReaxChildren
{
    public ReaxNode[] Context => Block.Context.ArrayConcat(condition);

    public ReaxNode[] Children => Block.Context.ArrayConcat(condition);

    public Symbol[] GetParameters(Guid scope)
        => [];

    public override string ToString()
    {
        return $"while {condition} {{...}}";
    }
}
