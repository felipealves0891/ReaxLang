using Reax.Interpreter;
using Reax.Semantic.Interfaces;
using Reax.Semantic.Symbols;

namespace Reax.Parser.Node;

public record ScriptNode(
    string Identifier, 
    ReaxInterpreter Interpreter, 
    SourceLocation Location) : ReaxNode(Location), IReaxExtensionContext
{
    public ReaxNode[] Context 
        => Interpreter.Nodes;

    public override string ToString()
    {
        return $"import script {Identifier};";
    }
}