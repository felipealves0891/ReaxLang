using Reax.Core.Locations;
using Reax.Core.Ast.Interfaces;
using Reax.Core.Ast.Literals;

namespace Reax.Core.Ast.Operations;

public record TermNode(
    string Operator, 
    SourceLocation Location) : ReaxNode(Location), IArithmeticOperator
{
    public override IReaxNode[] Children => [];

    public NumberNode Calculate(NumberNode x, NumberNode y)
    {
        var left = Convert.ToDecimal(x.Value);
        var rigth = Convert.ToDecimal(y.Value);

        return Operator switch
        {
            "+" => new NumberNode((left + rigth).ToString(), x.Location),
            "-" => new NumberNode((left - rigth).ToString(), x.Location),
            _ => throw new InvalidOperationException("Operador invalido!")
        };
    }

    public override string ToString()
    {
        return Operator.ToString();
    }
}
