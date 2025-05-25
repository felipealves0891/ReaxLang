using Reax.Core.Ast.Expressions;
using Reax.Core.Ast.Literals;
using Reax.Core.Locations;


namespace Reax.Core.Ast.Statements;

public record ForNode(
    DeclarationNode Declaration, 
    ReaxNode Condition, 
    ContextNode Block, 
    SourceLocation Location) : StatementNode(Location), IBranchFlowNode, IReaxDeclaration
{
    public override IReaxNode[] Children => [Declaration, Condition, Block];

    public void Initialize(IReaxExecutionContext context)
    {
        Declaration.Initialize(context);
    }

    public override void Execute(IReaxExecutionContext context)
    {
        var condition = (BinaryNode)Condition;
        Declaration.Execute(context);
        while ((bool)condition.Evaluation(context).Value)
        {
            var interpreter = context.CreateInterpreter(ToString(), Block.Block);
            interpreter.Interpret();

            var value = context.GetVariable(Declaration.Identifier) as NumberNode;
            if (value is null)
                throw new InvalidOperationException("Não foi possivel obter o controlador do loop");

            var intValue = Convert.ToInt32(value.Value) + 1;
            var newValue = new NumberNode(intValue.ToString(), Declaration.Location);
            context.SetVariable(Declaration.Identifier, newValue);
        }
    }

    public bool HasGuaranteedReturn()
    {
        return Block.HasGuaranteedReturn();
    }

    public override string ToString()
    {
        return $"for {Declaration} to {Condition} {{...}}";
    }
}
