using Reax.Parser.Node.Interfaces;
using Reax.Runtime;

namespace Reax.Parser.Node;

public record ReturnNode(
    ReaxNode Expression, 
    SourceLocation Location) : ReaxNode(Location), IReaxResultType
{
    public SymbolType GetDataType()
    {
        if(Expression is IReaxResultType type)
            return type.GetDataType();
        else
            return SymbolType.NONE;
    }

    public override string ToString()
    {
        return $"return {Expression}";
    }
}
