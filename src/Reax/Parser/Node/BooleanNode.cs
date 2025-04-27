using Reax.Parser.Node.Interfaces;
using Reax.Runtime;

namespace Reax.Parser.Node;

public record BooleanNode(string Value, 
    SourceLocation Location) : ReaxNode(Location), IReaxValue
{
    public object ValueConverted => bool.Parse(Value.ToLower());

    public override string ToString()
    {
        return $"{Value.ToLower()}";
    }
}
