using Reax.Parser.Node.Interfaces;
using Reax.Runtime;

namespace Reax.Parser.Node;

public record VarNode(
    string Identifier, 
    DataType Type,
    SourceLocation Location) : ReaxNode(Location), IReaxValue
{
    public object Value => Identifier;

    public override string ToString()
    {
        return Identifier;
    }
}
