using System;
using Reax.Core.Ast.Interfaces;
using Reax.Core.Ast.Literals;

namespace Reax.Core.Ast;

public class ReturnSuccessException : Exception
{
    public ReturnSuccessException(IReaxValue value)
    {
        Value = value;
    }

    public IReaxValue Value { get; }
}
