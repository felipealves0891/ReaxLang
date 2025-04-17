namespace Reax.Parser.Node.Interfaces;

public interface ILogicOperator : IOperator
{
    bool Compare(ReaxNode x, ReaxNode y);
};
