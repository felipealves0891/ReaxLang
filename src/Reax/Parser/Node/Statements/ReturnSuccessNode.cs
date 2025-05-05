using Reax.Parser.Node.Interfaces;
using Reax.Semantic;

namespace Reax.Parser.Node.Statements;

public record ReturnSuccessNode(
    ReaxNode Expression, 
    SourceLocation Location) : StatementNode(Location), IControlFlowNode
{
    public override IReaxNode[] Children => [Expression];

    public bool HasGuaranteedReturn()
    {
        return true;
    }

    public override string ToString()
    {
        return $"return success {Expression}";
    }
}
