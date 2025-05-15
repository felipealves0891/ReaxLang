using Reax.Core.Ast.Expressions;
using Reax.Core.Locations;
using Reax.Core.Types;

namespace Reax.Core.Ast.Statements;

public record ActionNode(
    VarNode[] Parameters,
    ReaxNode Context,
    DataType Type,
    SourceLocation Location) : StatementNode(Location), IBranchFlowNode
{
    public override IReaxNode[] Children => [Context];

    public override void Accept(IReaxInterpreter interpreter)
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
        var parameters = string.Join(',', Parameters.Select(x => x.ToString()));
        return $"({parameters}){Type} -> {{...}}";
    }
}
