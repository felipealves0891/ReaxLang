using Reax.Parser.Node.Expressions;
using Reax.Parser.Node.Interfaces;
using Reax.Semantic;

namespace Reax.Parser.Node;

public record VarNode(
    string Identifier, 
    DataType Type,
    SourceLocation Location) : ExpressionNode(Location), IReaxValue
{
    public object Value => Identifier;
    public override IReaxNode[] Children => [];

    public override string ToString()
    {
        return Identifier;
    }
}
