using Reax.Core.Ast.Literals;

namespace Reax.Core.Ast.Interfaces;

public interface IArithmeticOperator : IOperator
{
    NumberNode Calculate(NumberNode x, NumberNode y);
};
