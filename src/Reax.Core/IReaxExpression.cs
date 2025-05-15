using System;
using Reax.Core.Ast.Literals;

namespace Reax.Core;

public interface IReaxExpression
{
    LiteralNode Evaluation(IReaxExecutionContext context);
}
