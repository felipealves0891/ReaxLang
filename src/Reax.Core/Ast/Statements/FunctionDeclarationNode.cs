using Reax.Core.Ast.Expressions;
using Reax.Core.Locations;
using Reax.Core.Types;


namespace Reax.Core.Ast.Statements;

public record FunctionDeclarationNode(
    string Identifier, 
    ContextNode Block, 
    VarNode[] Parameters, 
    DataType SuccessType,
    DataType ErrorType,
    SourceLocation Location) : StatementNode(Location), IControlFlowNode, IReaxDeclaration
{
    public override IReaxNode[] Children => Parameters.Cast<ReaxNode>().Concat(Block.Block.Cast<ReaxNode>()).ToArray();
    
    public DataType ResultSuccess => SuccessType;

    public DataType ResultError => ErrorType;

    public void Initialize(IReaxExecutionContext context)
    {
        context.Declare(Identifier);
    }

    public override void Execute(IReaxExecutionContext context)
    {
        var interpreter = context.CreateInterpreter(ToString(), Block.Block, Parameters);
        context.SetFunction(Identifier, interpreter);
    }

    public bool HasGuaranteedReturn()
    {
        return Block.HasGuaranteedReturn();
    }

    public override string ToString()
    {
        var param = string.Join(',', Parameters.Select(x => x.ToString()));
        return $"fun {Identifier} ({param}):{SuccessType} | {ErrorType} {{...}}";
    }
}
