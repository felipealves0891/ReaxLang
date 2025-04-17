namespace Reax.Parser.Node;

public record ModuleFunctionCallNode(string moduleName, FunctionCallNode functionCall) : ReaxNode;
