using System;
using Reax.Parser.Node.Interfaces;
using Reax.Semantic.Interfaces;
using Reax.Semantic.Symbols;

namespace Reax.Parser.Node;

public record BindNode(
    string Identifier, 
    ReaxNode Node,
    DataTypeNode DataType, 
    SourceLocation Location) : ReaxNode(Location), IReaxDeclaration, IReaxAssignment
{
    public IReaxType TypeAssignedValue => (IReaxType)Node;

    public Symbol GetSymbol(Guid scope)
    {
        return new Symbol(
            Identifier,
            DataType.TypeName,
            SymbolCategoty.BIND,
            scope);
    }

    public override string ToString()
    {
        return $"bind {Identifier}: {DataType} -> {{...}}";
    }
}
