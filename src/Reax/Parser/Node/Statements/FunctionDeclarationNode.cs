using Reax.Parser.Node.Interfaces;
using Reax.Parser.Node.Statements;

namespace Reax.Parser.Node;

public record FunctionDeclarationNode(
    IdentifierNode Identifier, 
    ContextNode Block, 
    VarNode[] Parameters, 
    DataType SuccessType,
    DataType ErrorType,
    SourceLocation Location) : ReaxNode(Location)
{
    public override string ToString()
    {
        var param = string.Join(',', Parameters.Select(x => x.ToString()));
        return $"fun {Identifier.Identifier} ({param}):{SuccessType} | {ErrorType} {{...}}";
    }
}
