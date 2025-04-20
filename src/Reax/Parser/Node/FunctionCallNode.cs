using Reax.Parser.Node.Interfaces;
using Reax.Runtime;

namespace Reax.Parser.Node;

public record FunctionCallNode(
    string Identifier, 
    ReaxNode[] Parameter, 
    SourceLocation Location) : ReaxNode(Location), IReaxResultType
{
    public SymbolType GetDataType()
    {
        return ReaxEnvironment.Symbols.Exists(Identifier)
            ? ReaxEnvironment.Symbols[Identifier].Type
            : SymbolType.NONE;
    }

    public override string ToString()
    {
        return $"{Identifier}({string.Join(',', Parameter.Select(x => x.ToString()))});";
    }
}
