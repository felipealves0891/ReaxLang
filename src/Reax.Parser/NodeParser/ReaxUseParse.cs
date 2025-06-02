using System;
using Reax.Core.Ast;
using Reax.Core.Ast.Expressions;
using Reax.Core.Locations;
using Reax.Core.Types;
using Reax.Lexer;
using Reax.Parser.Extensions;
using Reax.Parser.Helper;

namespace Reax.Parser.NodeParser;

public class ReaxUseInstanceParse : INodeParser
{
    public bool IsParse(Token before, Token current, Token next)
    {
        return current.Type == TokenType.USE;
    }

    public ReaxNode? Parse(ITokenSource source)
    {
        return NativeCallHelper.Parser(source.NextExpression().ToArray());
    }
}
