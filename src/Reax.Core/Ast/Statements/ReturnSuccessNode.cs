using System.Diagnostics.CodeAnalysis;
using Reax.Core.Locations;
using Reax.Core.Ast.Interfaces;


namespace Reax.Core.Ast.Statements;

[ExcludeFromCodeCoverage]
public record ReturnSuccessNode(
    ReaxNode Expression, 
    SourceLocation Location) : StatementNode(Location), IBranchFlowNode
{
    public override IReaxNode[] Children => [Expression];

    public override void Accept(IReaxInterpreter interpreter)
    {
        throw new NotImplementedException();
    }

    public bool HasGuaranteedReturn()
    {
        return true;
    }

    public override string ToString()
    {
        return $"return success {Expression}";
    }
}
