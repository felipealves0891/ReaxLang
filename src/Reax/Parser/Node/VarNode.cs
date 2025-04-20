using Reax.Parser.Node.Interfaces;
using Reax.Runtime;

namespace Reax.Parser.Node;

public record VarNode(
    string Identifier, 
    SourceLocation Location) : ReaxNode(Location), IReaxValue
{
    public SymbolType GetDataType()
    {
        return ReaxEnvironment.Symbols.Exists(Identifier)
             ? ReaxEnvironment.Symbols[Identifier].Type
             : SymbolType.NONE;
    }

    public override string ToString()
    {
        return Identifier;
    }
}
