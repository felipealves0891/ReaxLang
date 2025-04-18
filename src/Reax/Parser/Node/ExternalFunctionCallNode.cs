namespace Reax.Parser.Node;

public record ExternalFunctionCallNode(string scriptName, FunctionCallNode functionCall) : ReaxNode;
