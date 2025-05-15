using System;
using Reax.Core.Ast;
using Reax.Core.Ast.Literals;

namespace Reax.Core;

public interface IReaxExpression
{
    LiteralNode Evaluate(IReaxInterpreter interpreter);
}
