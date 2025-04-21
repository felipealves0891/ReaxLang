using Reax.Parser.Node.Interfaces;
using Reax.Runtime;

namespace Reax.Parser.Node;

public record NumberNode(
    string Value, 
    SourceLocation Location) : ReaxNode(Location), IReaxValue
{
    public decimal ValueConverted => decimal.Parse(Value);
    
    public override string ToString()
    {
        return $"{Value}";
    }
}
