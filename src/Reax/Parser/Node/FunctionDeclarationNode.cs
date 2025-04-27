using Reax.Parser.Node.Interfaces;

namespace Reax.Parser.Node;

public record FunctionDeclarationNode(
    IdentifierNode Identifier, 
    ContextNode Block, 
    VarNode[] Parameters, 
    DataTypeNode SuccessType,
    DataTypeNode ErrorType,
    SourceLocation Location) : ReaxNode(Location)
{
    public override string ToString()
    {
        var param = string.Join(',', Parameters.Select(x => x.ToString()));
        return $"fun {Identifier.Identifier} ({param}){SuccessType} {{...}}";
    }
}
