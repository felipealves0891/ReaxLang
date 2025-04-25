using Reax.Parser.Node.Interfaces;

namespace Reax.Parser.Node;

public record ComparisonNode(string Operator, 
    SourceLocation Location) : ReaxNode(Location), ILogicOperator
{
    public bool Compare(ReaxNode x, ReaxNode y)
    {
        var left = (NumberNode)x;
        var rigth = (NumberNode)y;

        if(Operator == "<")
            return (decimal)left.ValueConverted < (decimal)rigth.ValueConverted;
        else if(Operator == ">")
            return (decimal)left.ValueConverted > (decimal)rigth.ValueConverted;
        else if(Operator == "<=")
            return (decimal)left.ValueConverted <= (decimal)rigth.ValueConverted;
        else if(Operator == ">=")
            return (decimal)left.ValueConverted > (decimal)rigth.ValueConverted;
        else
            throw new InvalidOperationException($"Operador de comparação invalido: {Operator}");
    }

    public override string ToString()
    {
        return Operator.ToString();
    }
}
