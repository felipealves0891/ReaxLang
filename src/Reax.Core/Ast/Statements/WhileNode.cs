using Reax.Core.Locations;


namespace Reax.Core.Ast.Statements;

public record WhileNode(
    ReaxNode Condition, 
    ContextNode Block, 
    SourceLocation Location) : StatementNode(Location), IBranchFlowNode
{
    public override IReaxNode[] Children => [Condition, Block];

    public override void Accept(IReaxInterpreter interpreter)
    {
        throw new NotImplementedException();
    }

    public bool HasGuaranteedReturn()
    {
        return Block.HasGuaranteedReturn();
    }

    public override string ToString()
    {
        return $"while {Condition} {{...}}";
    }
}
