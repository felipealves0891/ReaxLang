namespace Reax.Parser.Node;

public record ScriptFunctionCallNode(string scriptName, FunctionCallNode functionCall) : ReaxNode;
