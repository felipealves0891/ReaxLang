using Reax.Parser.Node.Interfaces;

namespace Reax.Parser.Node;

public record ObservableNode(ReaxNode Var, ReaxNode Block, BinaryNode? Condition, 
    SourceLocation Location) : ReaxNode(Location)
{
    
    public override string ToString()
    {
        return $"on {Var} {{...}}";
    }
}
