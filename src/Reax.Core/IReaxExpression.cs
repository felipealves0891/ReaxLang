using System;
using Reax.Core.Ast.Interfaces;
using Reax.Core.Ast.Literals;

namespace Reax.Core;

public interface IReaxExpression
{
    IReaxValue Evaluation(IReaxExecutionContext context);
}
