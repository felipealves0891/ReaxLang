using Reax.Interpreter;

namespace Reax.Parser.Node;

public record ScriptNode(string Identifier, ReaxInterpreter Interpreter) : ReaxNode
{
    public override string ToString()
    {
        return $"import script {Identifier};";
    }
}