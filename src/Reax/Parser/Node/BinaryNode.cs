using Reax.Parser.Node.Interfaces;
using Reax.Runtime;

namespace Reax.Parser.Node;

public record BinaryNode(
    ReaxNode Left, 
    ReaxNode Operator, 
    ReaxNode Right, 
    SourceLocation Location) : ReaxNode(Location), IReaxType
{
    public DataType Type => DataType.BOOLEAN;

    public override string ToString()
    {
        return $"{Left} {Operator} {Right}";
    }
}
