using Reax.Parser.Node.Interfaces;
using Reax.Parser.Node.Literals;

namespace Reax.Parser.Node.Operations;

public record ComparisonNode(
    string Operator, 
    SourceLocation Location) : ReaxNode(Location), ILogicOperator
{
    public bool Compare(ReaxNode x, ReaxNode y)
    {
        var left = (NumberNode)x;
        var rigth = (NumberNode)y;

        if(Operator == "<")
            return (decimal)left.Value < (decimal)rigth.Value;
        else if(Operator == ">")
            return (decimal)left.Value > (decimal)rigth.Value;
        else if(Operator == "<=")
            return (decimal)left.Value <= (decimal)rigth.Value;
        else if(Operator == ">=")
            return (decimal)left.Value > (decimal)rigth.Value;
        else
            throw new InvalidOperationException($"Operador de comparação invalido: {Operator}");
    }

    public override string ToString()
    {
        return Operator.ToString();
    }
}
