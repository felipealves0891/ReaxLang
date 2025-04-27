using Reax.Parser.Node.Interfaces;

namespace Reax.Parser.Node;

public record DeclarationNode(
    string Identifier, 
    bool Immutable, 
    bool Async, 
    DataTypeNode DataType,
    ReaxNode? Assignment, 
    SourceLocation Location) : ReaxNode(Location)
{
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
