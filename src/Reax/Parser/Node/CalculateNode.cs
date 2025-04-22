using Reax.Parser.Node.Interfaces;
using Reax.Runtime;
using Reax.Semantic.Interfaces;
using Reax.Semantic.Scopes;
using Reax.Semantic.Symbols;

namespace Reax.Parser.Node;

public record CalculateNode(
    ReaxNode Left, 
    ReaxNode Operator, 
    ReaxNode Right, 
    SourceLocation Location) : ReaxNode(Location), IReaxType
{
    public SymbolType GetReaxType(IReaxScope scope)
    {
        var leftType = ((IReaxType)Left).GetReaxType(scope);
        var rightType = ((IReaxType)Right).GetReaxType(scope);

        if(!leftType.IsCompatible(rightType))
            return SymbolType.NONE;

        return leftType.GetTypeNumberResult(rightType);
    }

    public override string ToString()
    {
        return $"{Left} {Operator} {Right}";
    }
}
