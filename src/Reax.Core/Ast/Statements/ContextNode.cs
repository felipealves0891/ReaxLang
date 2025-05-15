using Reax.Core.Locations;
using Reax.Core.Ast.Interfaces;
using Reax.Core.Ast.Statements;

namespace Reax.Core.Ast.Statements;

public record ContextNode(
    ReaxNode[] Block, 
    SourceLocation Location) : StatementNode(Location), IBranchFlowNode
{
    public override IReaxNode[] Children => Block;

    public override void Execute(IReaxExecutionContext context)
    {
        throw new NotImplementedException();
    }

    public bool HasGuaranteedReturn()
    {
        return Block.Any(x => x is IBranchFlowNode control ? control.HasGuaranteedReturn() : false);
    }

    public override string ToString()
    {
        return "{...}";
    }
}
