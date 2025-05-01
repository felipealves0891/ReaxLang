using Reax.Parser.Node.Interfaces;
using Reax.Runtime;

namespace Reax.Parser.Node.Expressions;

public record ExternalFunctionCallNode(
    string scriptName, 
    FunctionCallNode functionCall, 
    SourceLocation Location) : ReaxNode(Location)
{
    public override string ToString()
    {
        var parameters = functionCall.Parameter.Select(x => x.ToString());
        return $"{scriptName}.{functionCall.Identifier}({string.Join(',', parameters)})";
    }
}
