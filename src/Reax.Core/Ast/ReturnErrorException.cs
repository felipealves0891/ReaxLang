using System;
using Reax.Core.Ast.Literals;

namespace Reax.Core.Ast;

public class ReturnErrorException : Exception
{
    public ReturnErrorException(LiteralNode literal)
    {
        Literal = literal;
    }

    public LiteralNode Literal { get; }
}

