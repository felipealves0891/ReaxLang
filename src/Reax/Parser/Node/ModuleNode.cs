using Reax.Interpreter;

namespace Reax.Parser.Node;

public record ModuleNode(string Identifier, ReaxInterpreter Interpreter) : ReaxNode;