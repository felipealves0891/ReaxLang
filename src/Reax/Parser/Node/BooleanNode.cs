using Reax.Parser.Node.Interfaces;
using Reax.Runtime;

namespace Reax.Parser.Node;

public record BooleanNode(string Value, 
    SourceLocation Location) : ReaxNode(Location), IReaxValue
{
    public bool ValueConverted => bool.Parse(Value);

    public override string ToString()
    {
        return $"{Value}";
    }
}
