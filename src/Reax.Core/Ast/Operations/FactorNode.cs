using Reax.Core.Locations;
using Reax.Core.Ast.Interfaces;
using Reax.Core.Ast.Literals;

namespace Reax.Core.Ast.Operations;

public record FactorNode(
    string Operator, 
    SourceLocation Location) : ReaxNode(Location), IArithmeticOperator
{
    public override IReaxNode[] Children => [];

    public NumberNode Calculate(NumberNode x, NumberNode y)
    {
        return Operator switch 
        {
            "*" => new NumberNode(((decimal)x.Value * (decimal)y.Value).ToString(), Location),
            "/" => new NumberNode(((decimal)x.Value / (decimal)y.Value).ToString(), Location),
            _ => throw new InvalidOperationException("Operador invalido!")
        };
    }

    public override string ToString()
    {
        return Operator.ToString();
    }
}
