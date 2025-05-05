using Reax.Parser.Node.Interfaces;
using Reax.Parser.Node.Statements;
using Reax.Runtime;
using Reax.Semantic;

namespace Reax.Parser.Node.Statements;

public record ContextNode(
    ReaxNode[] Block, 
    SourceLocation Location) : StatementNode(Location), IControlFlowNode
{
    public override IReaxNode[] Children => Block;

    public bool HasGuaranteedReturn()
    {
        return Block.Any(x => x is IControlFlowNode control ? control.HasGuaranteedReturn() : false);
    }

    public override string ToString()
    {
        return "{...}";
    }
}
