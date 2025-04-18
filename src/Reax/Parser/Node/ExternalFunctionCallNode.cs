namespace Reax.Parser.Node;

public record ExternalFunctionCallNode(string scriptName, FunctionCallNode functionCall) : ReaxNode
{
    public override string ToString()
    {
        var parameters = functionCall.Parameter.Select(x => x.ToString());
        return $"{scriptName}.{functionCall.Identifier}({string.Join(',', parameters)})";
    }
}
