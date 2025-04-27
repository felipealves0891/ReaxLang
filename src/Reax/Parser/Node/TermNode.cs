using Reax.Parser.Node.Interfaces;
using Reax.Parser.Node.Literals;

namespace Reax.Parser.Node;

public record TermNode(
    string Operator, 
    SourceLocation Location) : ReaxNode(Location), IArithmeticOperator
{
    public NumberNode Calculate(NumberNode x, NumberNode y)
    {
        return Operator switch 
        {
            "+" => new NumberNode(((decimal)x.Value + (decimal)y.Value).ToString(), x.Location),
            "-" => new NumberNode(((decimal)x.Value - (decimal)y.Value).ToString(), x.Location),
            _ => throw new InvalidOperationException("Operador invalido!")
        };
    }

    public override string ToString()
    {
        return Operator.ToString();
    }
}
