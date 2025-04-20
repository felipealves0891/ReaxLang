using Reax.Parser.Node.Interfaces;
using Reax.Runtime;

namespace Reax.Parser.Node;

public record CalculateNode(
    ReaxNode Left, 
    ReaxNode Operator, 
    ReaxNode Right, 
    SourceLocation Location) : ReaxNode(Location), IReaxResultType
{
    public SymbolType GetDataType()
    {
        var leftResult = Left as IReaxResultType;
        var rightResult = Right as IReaxResultType;

        if(leftResult is null || rightResult is null)
            return SymbolType.NONE;

        if(leftResult.GetDataType() == rightResult.GetDataType())
            return leftResult.GetDataType();
        else if(leftResult.GetDataType().IsCompatible(rightResult.GetDataType()))
            return leftResult.GetDataType().GetTypeNumberResult(rightResult.GetDataType());
        else
            return SymbolType.NONE;
    }

    public override string ToString()
    {
        return $"{Left} {Operator} {Right}";
    }
}
