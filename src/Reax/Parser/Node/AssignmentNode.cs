using Reax.Parser.Node.Interfaces;
using Reax.Semantic.Interfaces;
using Reax.Semantic.Symbols;

namespace Reax.Parser.Node;

public record AssignmentNode(
    string Identifier, 
    ReaxNode Assigned, 
    SourceLocation Location) : ReaxNode(Location), IReaxContext, IReaxAssignment, IReaxChildren
{
    public ReaxNode[] Context => Assigned is ContextNode block ? block.Context : [Assigned];
    public IReaxType TypeAssignedValue => (IReaxType)Assigned;

    public ReaxNode[] Children => [Assigned];

    public Symbol[] GetParameters(Guid scope)
        => Assigned is IReaxContext context ? context.GetParameters(scope) : [];

    public override string ToString()
    {
        return $"{Identifier} = {Assigned};";
    }
}
