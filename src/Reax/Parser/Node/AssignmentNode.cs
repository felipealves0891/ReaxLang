using Reax.Parser.Node.Interfaces;
using Reax.Semantic.Interfaces;
using Reax.Semantic.Symbols;

namespace Reax.Parser.Node;

public record AssignmentNode(
    string Identifier, 
    ReaxNode Assigned, 
    SourceLocation Location) : ReaxNode(Location), IReaxContext
{
    public ReaxNode[] Context => Assigned is ContextNode block ? block.Context : [Assigned];

    public Symbol[] GetParameters(Guid scope)
        => [];

    public override string ToString()
    {
        return $"{Identifier} = {Assigned};";
    }
}
