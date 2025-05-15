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
    SourceLocation Location) : StatementNode(Location), IControlFlowNode
{
    public override IReaxNode[] Children => Parameters.Concat(Block.Block).ToArray();
    
    public DataType ResultSuccess => SuccessType;

    public DataType ResultError => ErrorType;

    public override void Execute(IReaxExecutionContext context)
    {
        throw new NotImplementedException();
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
