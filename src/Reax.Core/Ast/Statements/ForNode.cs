using Reax.Core.Locations;


namespace Reax.Core.Ast.Statements;

public record ForNode(
    DeclarationNode Declaration, 
    ReaxNode Condition, 
    ContextNode Block, 
    SourceLocation Location) : StatementNode(Location), IBranchFlowNode
{
    public override IReaxNode[] Children => [Declaration, Condition, Block];

    public bool HasGuaranteedReturn()
    {
        return Block.HasGuaranteedReturn();
    }

    public override string ToString()
    {
        return $"for {Declaration} to {Condition} {{...}}";
    }
}
