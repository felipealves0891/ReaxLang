using Reax.Semantic;

namespace Reax.Parser.Node.Statements;

public record ActionNode(
    VarNode[] Parameters,
    ReaxNode Context,
    DataType Type,
    SourceLocation Location) : StatementNode(Location), IBranchFlowNode
{
    public override IReaxNode[] Children => [Context];

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
