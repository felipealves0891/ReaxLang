using Reax.Parser.Node.Interfaces;

namespace Reax.Parser.Node;

public record VarNode(string Identifier, 
    SourceLocation Location) : ReaxNode(Location), IReaxValue
{
    public override string ToString()
    {
        return Identifier;
    }
}
