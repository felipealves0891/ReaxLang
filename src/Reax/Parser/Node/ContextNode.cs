using Reax.Parser.Node.Interfaces;
using Reax.Runtime;

namespace Reax.Parser.Node;

public record ContextNode(
    ReaxNode[] Block, 
    SourceLocation Location) : ReaxNode(Location), IReaxResultType
{
    public ReaxNode[] Context => Block;

    public SymbolType GetDataType()
    {
        foreach (var block in Block)
        {
            if(block is IReaxResultType resultType)
                return resultType.GetDataType();
        }

        return SymbolType.NONE;
    }

    public override string ToString()
    {
        return "{...}";
    }
}
