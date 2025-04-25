using Reax.Parser.Node.Interfaces;

namespace Reax.Parser.Node;

public record EqualityNode(
    string Operator, 
    SourceLocation Location) : ReaxNode(Location), ILogicOperator
{
    public bool Compare(ReaxNode x, ReaxNode y)
    {
        if(x is IReaxValue xValue && y is IReaxValue yValue)
            return Compare(xValue, yValue);
        else
            throw new InvalidOperationException("Equality esperava dois valores para comparar");
    }

    private bool Compare(IReaxValue x, IReaxValue y)
    {
        if(Operator == "==" )
            return x.ValueConverted.Equals(y.ValueConverted);
        else
            return !x.ValueConverted.Equals(y.ValueConverted);
    }

    public override string ToString()
    {
        return Operator.ToString();
    }
}
