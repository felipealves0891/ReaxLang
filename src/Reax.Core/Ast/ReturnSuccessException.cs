using System;
using Reax.Core.Ast.Literals;

namespace Reax.Core.Ast;

public class ReturnSuccessException : Exception
{
    public ReturnSuccessException(LiteralNode literal)
    {
        Literal = literal;
    }

    public LiteralNode Literal { get; }
}
