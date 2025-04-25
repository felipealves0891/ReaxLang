using Reax.Parser.Node.Interfaces;

namespace Reax.Parser.Node;

public record LogicNode(string Operator, 
    SourceLocation Location) : ReaxNode(Location), ILogicOperator
{
    public bool Compare(ReaxNode x, ReaxNode y)
    {
        var left = (BooleanNode)x;
        var right = (BooleanNode)y;

        return Operator switch 
        {
            "and" => (bool)left.ValueConverted && (bool)right.ValueConverted,
            "or" => (bool)left.ValueConverted || (bool)right.ValueConverted,
            _ => throw new InvalidOperationException($"Operador invalido para operação logica {Operator}!")
        };
    }

    public override string ToString()
    {
        return Operator.ToString();
    }
}
