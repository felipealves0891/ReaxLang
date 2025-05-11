using Reax.Core.Locations;
using Reax.Parser.Node.Interfaces;
using Reax.Parser.Node.Statements;
using Reax.Runtime;
using Reax.Semantic;

namespace Reax.Parser.Node.Statements;

public record ContextNode(
    ReaxNode[] Block, 
    SourceLocation Location) : StatementNode(Location), IBranchFlowNode
{
    public override IReaxNode[] Children => Block;

    public bool HasGuaranteedReturn()
    {
        return Block.Any(x => x is IBranchFlowNode control ? control.HasGuaranteedReturn() : false);
    }

    public override string ToString()
    {
        return "{...}";
    }
}
