using Reax.Parser.Node.Interfaces;

namespace Reax.Parser.Node;

public record VarNode(string Identifier) : ReaxNode, IReaxValue
{
    public override string ToString()
    {
        return Identifier;
    }
}
