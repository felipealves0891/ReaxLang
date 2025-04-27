using Reax.Parser.Node.Interfaces;
using Reax.Runtime;

namespace Reax.Parser.Node;

public record VarNode(
    string Identifier, 
    DataTypeNode DataType,
    SourceLocation Location) : ReaxNode(Location), IReaxValue
{
    public object Value => Identifier;
    public DataType Type => Enum.Parse<DataType>(DataType.TypeName);

    public override string ToString()
    {
        return Identifier;
    }
}
