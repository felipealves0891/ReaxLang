using Reax.Parser.Node.Interfaces;

namespace Reax.Parser.Node;

public record EqualityNode(string Operator) : ReaxNode, ILogicOperator
{
    public bool Compare(ReaxNode x, ReaxNode y)
    {
        return Operator == "==" 
             ? x.ToString() == y.ToString()
             : x.ToString() != y.ToString();
    }

    public override string ToString()
    {
        return Operator.ToString();
    }
}
