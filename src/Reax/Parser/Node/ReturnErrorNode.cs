using Reax.Parser.Node.Interfaces;
using Reax.Runtime;

namespace Reax.Parser.Node;

public record ReturnErrorNode(
    ReaxNode Expression, 
    SourceLocation Location) : ReaxNode(Location)
{
    public override string ToString()
    {
        return $"return error {Expression}";
    }
}
