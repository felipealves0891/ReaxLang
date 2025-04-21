using Reax.Parser.Node.Interfaces;
using Reax.Semantic.Interfaces;
using Reax.Semantic.Symbols;

namespace Reax.Parser.Node;

public record FunctionDeclarationNode(
    ReaxNode Identifier, 
    ContextNode Context, 
    ReaxNode[] Parameters, 
    DataTypeNode DataType,
    SourceLocation Location) : ReaxNode(Location), IReaxDeclaration
{
    public Symbol GetSymbol(Guid scope)
    {
        return new Symbol(
            Identifier.ToString(),
            DataType.TypeName,
            SymbolCategoty.FUNCTION,
            scope
        );
    }

    public override string ToString()
    {

        var param = string.Join(',', Parameters.Select(x => x.ToString()));
        return $"fun {Identifier} ({param}): {DataType} {{...}}";
    }
}
