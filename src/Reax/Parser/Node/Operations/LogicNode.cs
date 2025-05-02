using Reax.Parser.Node.Interfaces;
using Reax.Parser.Node.Literals;
using Reax.Semantic;

namespace Reax.Parser.Node.Operations;

public record LogicNode(string Operator, 
    SourceLocation Location) : ReaxNode(Location), ILogicOperator
{
    public override IReaxNode[] Children => [];

    public bool Compare(ReaxNode x, ReaxNode y)
    {
        var left = (BooleanNode)x;
        var right = (BooleanNode)y;

        return Operator switch 
        {
            "and" => (bool)left.Value && (bool)right.Value,
            "or" => (bool)left.Value || (bool)right.Value,
            _ => throw new InvalidOperationException($"Operador invalido para operação logica {Operator}!")
        };
    }

    public override string ToString()
    {
        return Operator.ToString();
    }
}
