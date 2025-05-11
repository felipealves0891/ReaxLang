using Reax.Core.Locations;
using Reax.Parser.Node.Interfaces;
using Reax.Semantic;

namespace Reax.Parser.Node.Operations;

public record EqualityNode(
    string Operator, 
    SourceLocation Location) : ReaxNode(Location), ILogicOperator
{
    public override IReaxNode[] Children => [];

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
            return x.Value.Equals(y.Value);
        else
            return !x.Value.Equals(y.Value);
    }

    public override string ToString()
    {
        return Operator.ToString();
    }
}
