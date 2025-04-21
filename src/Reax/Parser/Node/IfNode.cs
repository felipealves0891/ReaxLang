using Reax.Parser.Node.Interfaces;

namespace Reax.Parser.Node;

public record IfNode(
    BinaryNode Condition, 
    ReaxNode True, 
    ReaxNode? False, 
    SourceLocation Location) : ReaxNode(Location)
{
    public override string ToString()
    {
        var elseText = False is null ? "" : "else {}";
        return $"if {Condition} {{}} {elseText}";
    }
}
