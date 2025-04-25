using Reax.Parser.Node.Interfaces;
using Reax.Semantic.Interfaces;
using Reax.Semantic.Symbols;

namespace Reax.Parser.Node;

public record DeclarationNode(
    string Identifier, 
    bool Immutable, 
    bool Async, 
    DataTypeNode DataType,
    ReaxNode? Assignment, 
    SourceLocation Location) : ReaxNode(Location), IReaxDeclaration, IReaxAssignment, IReaxContext, IReaxChildren
{
    public IReaxType TypeAssignedValue => Assignment is null ? new NullNode(Location) : (IReaxType)Assignment;

    public ReaxNode[] Context => Assignment is IReaxContext context ? context.Context : [Assignment ?? new NullNode(Location)];

    public ReaxNode[] Children => Assignment is null ? [DataType] : [DataType, Assignment];

    public Symbol[] GetParameters(Guid scope)
        => Assignment is IReaxContext context ? context.GetParameters(scope) : [];

    public Symbol GetSymbol(Guid scope, string? module = null)
    {
        return new Symbol(
            Identifier, 
            DataType.TypeName,
            Immutable ? SymbolCategoty.CONST : SymbolCategoty.LET,
            scope,
            !Immutable,
            Immutable,
            Async,
            parentName: module);
    }

    public override string ToString()
    {
        var asc = Async ? "async " : "";
        var mut = Immutable ? "const" : "let";
        if(Assignment is not null)
            return $"{asc}{mut} {Identifier}{DataType} = {Assignment};";
        else 
            return $"{asc}{mut} {Identifier}{DataType};";
    }
}
