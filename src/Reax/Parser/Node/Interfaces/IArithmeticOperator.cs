using Reax.Parser.Node.Literals;

namespace Reax.Parser.Node.Interfaces;

public interface IArithmeticOperator : IOperator
{
    NumberNode Calculate(NumberNode x, NumberNode y);
};
