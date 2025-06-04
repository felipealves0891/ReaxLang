using System;
using Reax.Core.Ast;
using Reax.Core.Ast.Objects;
using Reax.Lexer;

namespace Reax.Parser.NodeParser;

public class ReaxInvokableParse : INodeParser
{
    public bool IsParse(Token before, Token current, Token next)
    {
        return current.Type == TokenType.INVOKABLE;
    }

    public ReaxNode? Parse(ITokenSource source)
    {
        var keyword = source.CurrentToken;
        source.Advance();
        var node = source.NextNode() ?? throw new InvalidOperationException("Era esperado um instrução ou expressão após um invokable!");
        return new InvokableNode(node, keyword.Location);
    }
}
