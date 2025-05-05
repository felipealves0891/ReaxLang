using Reax.Semantic;

namespace Reax.Parser.Node.Statements;

public record FunctionDeclarationNode(
    string Identifier, 
    ContextNode Block, 
    VarNode[] Parameters, 
    DataType SuccessType,
    DataType ErrorType,
    SourceLocation Location) : StatementNode(Location), IControlFlowNode
{
    public override IReaxNode[] Children => Parameters.Concat(Block.Block).ToArray();

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
