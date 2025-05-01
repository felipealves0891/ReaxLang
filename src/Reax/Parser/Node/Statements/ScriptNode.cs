using Reax.Interpreter;
using Reax.Parser.Node.Interfaces;

namespace Reax.Parser.Node;

public record ScriptNode(
    string Identifier, 
    ReaxInterpreter Interpreter, 
    SourceLocation Location) : ReaxNode(Location)
{
    public override string ToString()
    {
        return $"import script {Identifier};";
    }
}