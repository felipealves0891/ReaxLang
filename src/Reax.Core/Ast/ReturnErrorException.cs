using System;
using Reax.Core.Ast.Literals;
using Reax.Core.Types;

namespace Reax.Core.Ast;

public class ReturnErrorException : Exception
{
    public LiteralNode Literal { get; init; }

    public ReturnErrorException(LiteralNode literal)
    {
        Literal = literal;
    }
}
