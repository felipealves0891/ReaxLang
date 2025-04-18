using Reax.Parser.Node.Interfaces;

namespace Reax.Parser.Node;

public record StringNode(string Value, 
    SourceLocation Location) : ReaxNode(Location), IReaxValue
{
    public override string ToString()
    {
        return $"'{Value}'";
    }
}
