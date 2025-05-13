using System.Diagnostics.CodeAnalysis;
using Reax.Core.Locations;
using Reax.Core.Ast.Interfaces;

namespace Reax.Core.Ast.Expressions;

[ExcludeFromCodeCoverage]
public record ExternalFunctionCallNode(
    string scriptName, 
    FunctionCallNode functionCall, 
    SourceLocation Location) : ExpressionNode(Location)
{
    public override IReaxNode[] Children => [];

    public override string ToString()
    {
        var parameters = functionCall.Parameter.Select(x => x.ToString());
        return $"{scriptName}.{functionCall.Identifier}({string.Join(',', parameters)})";
    }
}
