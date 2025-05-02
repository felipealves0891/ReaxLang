using Reax.Interpreter;
using Reax.Parser.Node.Interfaces;
using Reax.Semantic;

namespace Reax.Parser.Node;

public record ScriptNode(
    string Identifier, 
    ReaxInterpreter Interpreter, 
    SourceLocation Location) : ReaxNode(Location)
{
    public override IReaxNode[] Children => Interpreter.Nodes;

    public override string ToString()
    {
        return $"import script {Identifier};";
    }
}