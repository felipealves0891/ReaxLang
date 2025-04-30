using Reax.Interpreter;
using Reax.Parser.Node.Interfaces;
using Reax.Semantic.Nodes;

namespace Reax.Parser.Node;

public record ScriptNode(
    string Identifier, 
    ReaxInterpreter Interpreter, 
    SourceLocation Location) : ReaxNode(Location), INode
{
    public bool IsLeaf => false;
    public INode[] Children => Interpreter.Nodes.Cast<INode>().ToArray();

    public override string ToString()
    {
        return $"import script {Identifier};";
    }
}