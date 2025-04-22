using Reax.Parser.Helper;
using Reax.Parser.Node.Interfaces;
using Reax.Semantic.Interfaces;
using Reax.Semantic.Symbols;

namespace Reax.Parser.Node;

public record ForNode(
    DeclarationNode Declaration, 
    ReaxNode Condition, 
    ContextNode Block, 
    SourceLocation Location) : ReaxNode(Location), IReaxContext, IReaxChildren
{
    public ReaxNode[] Context 
        => Block.Context.ArrayConcat(Condition, Declaration);

    public ReaxNode[] Children => [Declaration, Condition, Block];

    public Symbol[] GetParameters(Guid scope)
        => [];

    public Symbol GetSymbol(Guid scope)
    {
        return Declaration.GetSymbol(scope);
    }

    public override string ToString()
    {
        return $"for {Declaration} to {Condition} {{...}}";
    }
}
