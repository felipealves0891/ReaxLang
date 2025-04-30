using Reax.Parser.Node.Interfaces;
using Reax.Semantic.Analyzers.TypeChecking;
using Reax.Semantic.Node;

namespace Reax.Parser.Node;

public record DeclarationNode(
    string Identifier, 
    bool Immutable, 
    bool Async, 
    DataType Type,
    ReaxNode? Assignment, 
    SourceLocation Location) : ReaxNode(Location), INodeExpectedType
{
    public MultiType ExpectedType => new MultiType(Type, Type);
    public bool IsLeaf => Assignment is null;
    public INode[] Children => Assignment is null ? [] : [(INode)Assignment];

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
