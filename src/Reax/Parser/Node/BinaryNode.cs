using Reax.Parser.Node.Interfaces;
using Reax.Runtime;
using Reax.Semantic.Interfaces;
using Reax.Semantic.Scopes;
using Reax.Semantic.Symbols;

namespace Reax.Parser.Node;

public record BinaryNode(
    ReaxNode Left, 
    ReaxNode Operator, 
    ReaxNode Right, 
    SourceLocation Location) : ReaxNode(Location), IReaxType, IReaxChildren
{
    public ReaxNode[] Children => [Left, Operator, Right];

    public SymbolType GetReaxType(IReaxScope scope)
    {
        return SymbolType.BOOL;
    }

    public override string ToString()
    {
        return $"{Left} {Operator} {Right}";
    }
}
