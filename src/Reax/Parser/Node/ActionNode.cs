using System;
using Reax.Semantic.Interfaces;
using Reax.Semantic.Scopes;
using Reax.Semantic.Symbols;

namespace Reax.Parser.Node;

public record ActionNode(
    VarNode[] Parameters,
    ReaxNode Context,
    DataTypeNode DataType,
    SourceLocation Location) : ReaxNode(Location), IReaxType, IReaxChildren, IReaxMultipleDeclaration
{
    private string _name = $"Action_{Guid.NewGuid()}";

    public ReaxNode[] Children => [Context];

    public SymbolType? GetReaxErrorType(IReaxScope scope) 
        => null;

    public SymbolType GetReaxType(IReaxScope scope)
        => Enum.TryParse<SymbolType>(DataType.TypeName, true, out var type)
            ? type
            : ((IReaxType)Context).GetReaxType(scope);

    public Symbol GetSymbol(Guid scope, string? module = null)
    {
        return new Symbol(
            _name,
            DataType.TypeName,
            SymbolCategoty.FUNCTION,
            scope
        );
    }

    public Symbol[] GetSymbols(Guid scope)
    {
        var symbols = new Symbol[Parameters.Length];
        for (int i = 0; i < Parameters.Length; i++)
        {
            symbols[i] = new Symbol(
                $"{_name}_{Parameters[i].Identifier}",
                Parameters[i].DataType.TypeName,
                SymbolCategoty.PARAMETER,
                scope,
                parentName: _name
            );
        }

        return symbols;
    }

    public override string ToString()
    {
        var parameters = string.Join(',', Parameters.Select(x => x.ToString()));
        return $"({parameters}){DataType} -> {{...}}";
    }
}
