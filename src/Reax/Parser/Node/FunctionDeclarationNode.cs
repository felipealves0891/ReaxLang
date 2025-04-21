using Reax.Parser.Node.Interfaces;
using Reax.Semantic.Interfaces;
using Reax.Semantic.Symbols;

namespace Reax.Parser.Node;

public record FunctionDeclarationNode(
    ReaxNode Identifier, 
    ContextNode Block, 
    VarNode[] Parameters, 
    DataTypeNode DataType,
    SourceLocation Location) : ReaxNode(Location), IReaxDeclaration, IReaxContext
{
    public ReaxNode[] Context => Block.Context;

    public Symbol GetSymbol(Guid scope)
    {
        return new Symbol(
            Identifier.ToString(),
            DataType.TypeName,
            SymbolCategoty.FUNCTION,
            scope
        );
    }

    public Symbol[] GetParameters(Guid scope)
    {
        var symbols = new Symbol[Parameters.Length];
        for (int i = 0; i < Parameters.Length; i++)
        {
            symbols[i] = new Symbol(
                Parameters[i].Identifier,
                Parameters[i].DataType.TypeName,
                SymbolCategoty.PARAMETER,
                scope
            );
        }

        return symbols;
    }

    public override string ToString()
    {

        var param = string.Join(',', Parameters.Select(x => x.ToString()));
        return $"fun {Identifier} ({param}){DataType} {{...}}";
    }
}
