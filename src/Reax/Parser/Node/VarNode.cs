using Reax.Parser.Node.Interfaces;
using Reax.Runtime;
using Reax.Semantic.Interfaces;
using Reax.Semantic.Scopes;
using Reax.Semantic.Symbols;

namespace Reax.Parser.Node;

public record VarNode(
    string Identifier, 
    DataTypeNode DataType,
    SourceLocation Location) : ReaxNode(Location), IReaxValue, IReaxType
{
    public object ValueConverted => Identifier;

    public SymbolType GetReaxType(IReaxScope scope)
    {
        return scope.Get(Identifier).Type;
    }

    public override string ToString()
    {
        return Identifier;
    }
}
