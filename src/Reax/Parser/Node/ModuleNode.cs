using Reax.Parser.Node.Interfaces;
using Reax.Runtime.Functions;
using Reax.Semantic.Node;

namespace Reax.Parser.Node;

public record ModuleNode(
    string identifier, 
    Dictionary<string, Function> functions, 
    SourceLocation Location) : ReaxNode(Location), INode
{
    public bool IsLeaf => true;
    public INode[] Children => [];

    public override string ToString()
    {
        return $"import module {identifier};";
    }
}
