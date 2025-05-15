using System.Diagnostics.CodeAnalysis;
using Reax.Core.Locations;
using Reax.Core.Types;


namespace Reax.Core.Ast.Statements;

[ExcludeFromCodeCoverage]
public record DeclarationNode(
    string Identifier, 
    bool Immutable, 
    bool Async, 
    DataType Type,
    AssignmentNode? Assignment, 
    SourceLocation Location) : StatementNode(Location)
{
    public override IReaxNode[] Children => Assignment is not null ? [Assignment] : [];

    public override void Execute(IReaxExecutionContext context)
    {
        throw new NotImplementedException();
    }

    public override string ToString()
    {
        var asc = Async ? "async " : "";
        var mut = Immutable ? "const" : "let";
        if(Assignment is not null)
            return $"{asc}{mut} {Identifier}: {Type} = {Assignment};";
        else 
            return $"{asc}{mut} {Identifier}: {Type};";
    }
}
