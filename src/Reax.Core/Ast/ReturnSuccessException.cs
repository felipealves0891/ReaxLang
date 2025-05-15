using System;
using Reax.Core.Ast.Literals;

namespace Reax.Core.Ast;

public class ReturnSuccessException : Exception
{
    public LiteralNode Literal { get; init; }

    public ReturnSuccessException(LiteralNode literal)
    {
        Literal = literal;
    }
}
