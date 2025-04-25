using Reax.Parser.Node.Interfaces;

namespace Reax.Parser.Node;

public record FactorNode(
    string Operator, 
    SourceLocation Location) : ReaxNode(Location), IArithmeticOperator
{
    public NumberNode Calculate(NumberNode x, NumberNode y)
    {
        return Operator switch 
        {
            "*" => new NumberNode(((decimal)x.ValueConverted * (decimal)y.ValueConverted).ToString(), Location),
            "/" => new NumberNode(((decimal)x.ValueConverted / (decimal)y.ValueConverted).ToString(), Location),
            _ => throw new InvalidOperationException("Operador invalido!")
        };
    }

    public override string ToString()
    {
        return Operator.ToString();
    }
}
