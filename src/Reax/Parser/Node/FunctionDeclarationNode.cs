using Reax.Parser.Node.Interfaces;

namespace Reax.Parser.Node;

public record FunctionDeclarationNode(
    ReaxNode Identifier, 
    ContextNode Context, 
    ReaxNode[] Parameters, 
    DataTypeNode DataType,
    SourceLocation Location) : ReaxNode(Location), IReaxContext
{
    public ReaxNode[] Branchs => Context.Branchs;

    public override string ToString()
    {
        var param = string.Join(',', Parameters.Select(x => x.ToString()));
        return $"fun {Identifier} ({param}): {DataType} {{...}}";
    }
}
