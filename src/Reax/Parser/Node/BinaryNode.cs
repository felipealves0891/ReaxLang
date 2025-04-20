using Reax.Parser.Node.Interfaces;
using Reax.Runtime;

namespace Reax.Parser.Node;

public record BinaryNode(
    ReaxNode Left, 
    ReaxNode Operator, 
    ReaxNode Right, 
    SourceLocation Location) : ReaxNode(Location), IReaxResultType
{
    public SymbolType GetDataType() => SymbolType.BOOLEAN;

    public override string ToString()
    {
        return $"{Left} {Operator} {Right}";
    }
}
