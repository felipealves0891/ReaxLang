using Reax.Parser.Node.Interfaces;

namespace Reax.Parser.Node;

public record BooleanNode(string Value) : ReaxNode, IReaxValue
{
    public bool ValueConverted => bool.Parse(Value);

    public override string ToString()
    {
        return $"{Value}";
    }
}
