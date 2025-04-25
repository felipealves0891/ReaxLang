using Reax.Parser.Node.Interfaces;
using Reax.Runtime;
using Reax.Semantic.Interfaces;
using Reax.Semantic.Scopes;
using Reax.Semantic.Symbols;

namespace Reax.Parser.Node;

public record FunctionCallNode(
    string Identifier, 
    ReaxNode[] Parameter, 
    SourceLocation Location) : ReaxNode(Location), IReaxType, IReaxFunctionCall
{
    public IReaxType[] Parameters => Parameter.Cast<IReaxType>().ToArray();
    public string? Module => null;

    public SymbolType GetReaxType(IReaxScope scope)
    {
        return scope.Get(Identifier).Type;
    }

    public override string ToString()
    {
        return $"{Identifier}({string.Join(',', Parameter.Select(x => x.ToString()))});";
    }
}
