using Reax.Core.Ast.Expressions;
using Reax.Core.Locations;


namespace Reax.Core.Ast.Statements;

public record WhileNode(
    ReaxNode Condition, 
    ContextNode Block, 
    SourceLocation Location) : StatementNode(Location), IBranchFlowNode
{
    public override IReaxNode[] Children => [Condition, Block];

    public override void Execute(IReaxExecutionContext context)
    {
        var condition = (BinaryNode)Condition;

        while((bool)condition.Evaluation(context).Value)
        {
            var interpreter = context.CreateInterpreter(ToString(), Block.Block);
            interpreter.Interpret();
        }
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
