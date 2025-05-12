namespace Reax.Core.Ast.Interfaces;

public interface ILogicOperator : IOperator
{
    bool Compare(ReaxNode x, ReaxNode y);
};
