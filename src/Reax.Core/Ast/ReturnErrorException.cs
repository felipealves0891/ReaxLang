using System;
using Reax.Core.Ast.Interfaces;
using Reax.Core.Ast.Literals;

namespace Reax.Core.Ast;

public class ReturnErrorException : Exception
{
    public ReturnErrorException(IReaxValue value)
    {
        Value = value;
    }

    public IReaxValue Value { get; }
}

