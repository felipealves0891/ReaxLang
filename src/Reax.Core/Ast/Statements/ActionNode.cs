using Reax.Core.Ast.Expressions;
using Reax.Core.Locations;
using Reax.Core.Types;

namespace Reax.Core.Ast.Statements;

public record ActionNode(
    VarNode Parameter,
    ReaxNode Context,
    DataType Type,
    SourceLocation Location) : StatementNode(Location), IBranchFlowNode
{
    public override IReaxNode[] Children => [Context];

    public override void Execute(IReaxExecutionContext context)
    {
        throw new NotImplementedException();
    }

    public bool HasGuaranteedReturn()
    {
        if(Context is IBranchFlowNode control)
            return control.HasGuaranteedReturn();
        else
            return false;
    }

    public override string ToString()
    {
        return $"({Parameter}){Type} -> {{...}}";
    }
}
