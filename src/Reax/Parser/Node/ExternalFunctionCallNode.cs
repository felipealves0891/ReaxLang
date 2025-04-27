using Reax.Parser.Node.Interfaces;
using Reax.Runtime;
using Reax.Semantic.Interfaces;
using Reax.Semantic.Scopes;
using Reax.Semantic.Symbols;

namespace Reax.Parser.Node;

public record ExternalFunctionCallNode(
    string scriptName, 
    FunctionCallNode functionCall, 
    SourceLocation Location) : ReaxNode(Location), IReaxType, IReaxChildren, IReaxFunctionCall
{
    public ReaxNode[] Children => [functionCall];

    public string Identifier => functionCall.Identifier;
    public IReaxType[] Parameters => functionCall.Parameters;
    public string? Module => scriptName;

    public SymbolType? GetReaxErrorType(IReaxScope scope)
    {
        return scope.Get(functionCall.Identifier, scriptName).ErrorType;
    }

    public SymbolType GetReaxType(IReaxScope scope)
    {
        return scope.Get(functionCall.Identifier, scriptName).SuccessType;
    }

    public override string ToString()
    {
        var parameters = functionCall.Parameter.Select(x => x.ToString());
        return $"{scriptName}.{functionCall.Identifier}({string.Join(',', parameters)})";
    }
}
