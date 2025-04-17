using Reax.Parser.Node.Interfaces;

namespace Reax.Parser.Node;

public record LogicNode(string Operator) : ReaxNode, ILogicOperator
{
    public bool Compare(ReaxNode x, ReaxNode y)
    {
        var left = (BooleanNode)x;
        var right = (BooleanNode)y;

        return Operator switch 
        {
            "and" => left.ValueConverted && right.ValueConverted,
            "or" => left.ValueConverted || right.ValueConverted,
            _ => throw new InvalidOperationException($"Operador invalido para operação logica {Operator}!")
        };
    }

    public override string ToString()
    {
        return Operator.ToString();
    }
}
