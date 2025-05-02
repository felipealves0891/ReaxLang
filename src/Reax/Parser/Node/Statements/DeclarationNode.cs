using Reax.Semantic;

namespace Reax.Parser.Node.Statements;

public record DeclarationNode(
    string Identifier, 
    bool Immutable, 
    bool Async, 
    DataType Type,
    AssignmentNode? Assignment, 
    SourceLocation Location) : StatementNode(Location)
{
    public override IReaxNode[] Children => Assignment is not null ? [Assignment] : [];

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
