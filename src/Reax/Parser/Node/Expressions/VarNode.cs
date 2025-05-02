using Reax.Parser.Node.Expressions;
using Reax.Parser.Node.Interfaces;
using Reax.Semantic;

namespace Reax.Parser.Node;

public record VarNode : ExpressionNode, IReaxValue
{

    public VarNode(
        string identifier, 
        DataType type,
        SourceLocation location) : base(location)
    {
        Identifier = identifier;
        Type = type;
    }

    public object Value => Identifier;
    public override IReaxNode[] Children => [];

    public string Identifier { get; }
    public DataType Type { get; set; }
    
    public override string ToString()
    {
        return Identifier;
    }
}
