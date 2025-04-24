using Reax.Parser.Node.Interfaces;
using Reax.Semantic.Interfaces;
using Reax.Semantic.Symbols;

namespace Reax.Parser.Node;

public record FunctionDeclarationNode(
    IdentifierNode Identifier, 
    ContextNode Block, 
    VarNode[] Parameters, 
    DataTypeNode DataType,
    SourceLocation Location) : ReaxNode(Location), IReaxMultipleDeclaration, IReaxContext, IReaxChildren
{
    public ReaxNode[] Context => Block.Context;

    public ReaxNode[] Children => Block.Block;

    public Symbol GetSymbol(Guid scope)
    {
        return new Symbol(
            Identifier.Identifier,
            DataType.TypeName,
            SymbolCategoty.FUNCTION,
            scope
        );
    }

    public Symbol[] GetParameters(Guid scope)
        => [];

    public override string ToString()
    {
        var param = string.Join(',', Parameters.Select(x => x.ToString()));
        return $"fun {Identifier.Identifier} ({param}){DataType} {{...}}";
    }

    public Symbol[] GetSymbols(Guid scope)
    {
        var symbols = new Symbol[Parameters.Length];
        for (int i = 0; i < Parameters.Length; i++)
        {
            symbols[i] = new Symbol(
                $"{Identifier.Identifier}_{Parameters[i].Identifier}",
                Parameters[i].DataType.TypeName,
                SymbolCategoty.PARAMETER,
                scope,
                parentName: Identifier.Identifier
            );
        }

        return symbols;
    }
}
