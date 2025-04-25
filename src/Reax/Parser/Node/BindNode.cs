using System;
using Reax.Parser.Node.Interfaces;
using Reax.Semantic.Interfaces;
using Reax.Semantic.Symbols;

namespace Reax.Parser.Node;

public record BindNode(
    IdentifierNode Identifier, 
    ContextNode Node,
    DataTypeNode DataType, 
    SourceLocation Location) : ReaxNode(Location), IReaxDeclaration, IReaxAssignment, IReaxBinder, IReaxChildren
{
    public IReaxType TypeAssignedValue => (IReaxType)Node;
    public IReaxChildren Bound => (IReaxChildren)Node;

    public ReaxNode[] Children => [Node, Node];

    string IReaxAssignment.Identifier => Identifier.Identifier;

    string IReaxBinder.Identifier => Identifier.Identifier;

    public Symbol GetSymbol(Guid scope, string? module = null)
    {
        return new Symbol(
            Identifier.Identifier,
            DataType.TypeName,
            SymbolCategoty.BIND,
            scope,
            parentName: module);
    }

    public override string ToString()
    {
        return $"bind {Identifier}: {DataType} -> {{...}}";
    }
}
