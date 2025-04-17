using Reax.Parser.Node.Interfaces;

namespace Reax.Parser.Node;

public record StringNode(string Value) : ReaxNode, IReaxValue
{
    public override string ToString()
    {
        return $"'{Value}'";
    }
}
