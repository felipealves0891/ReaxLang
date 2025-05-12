using Reax.Core.Locations;
using Reax.Core.Ast.Expressions;


namespace Reax.Core.Ast.Statements;

public record IfNode(
    BinaryNode Condition, 
    ContextNode True, 
    ContextNode? False, 
    SourceLocation Location) : StatementNode(Location), IBranchFlowNode
{
    public override IReaxNode[] Children => False is null ? [Condition, True] : [Condition, True, False];

    public bool HasGuaranteedReturn()
    {
        var hasGuaranteedReturn = true;
        if(False is not null)
            hasGuaranteedReturn = False.HasGuaranteedReturn();

        return hasGuaranteedReturn && True.HasGuaranteedReturn();
    }

    public override string ToString()
    {
        var elseText = False is null ? "" : "else {}";
        return $"if {Condition} {{}} {elseText}";
    }
}
