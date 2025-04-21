using Reax.Parser.Node.Interfaces;
using Reax.Semantic.Interfaces;
using Reax.Semantic.Symbols;

namespace Reax.Parser.Node;

public record ForNode(
    DeclarationNode Declaration, 
    ReaxNode Condition, 
    ContextNode Block, 
    SourceLocation Location) : ReaxNode(Location), IReaxContext
{
    public ReaxNode[] Context => Block.Context;

    public Symbol[] GetParameters(Guid scope)
    {
        return [
            new Symbol(
                Declaration.Identifier,
                Declaration.DataType.TypeName,
                SymbolCategoty.PARAMETER,
                scope)
        ];
    }

    public override string ToString()
    {
        return $"for {Declaration} to {Condition} {{...}}";
    }
}
