using System;
using Reax.Core.Debugger;
using Reax.Lexer;
using Reax.Parser.Helper;
using Reax.Parser.Node;
using Reax.Core.Ast.Expressions;
using Reax.Core.Ast.Statements;
using Reax.Parser.NodeParser;
using Reax.Core.Ast;

namespace Reax.Core.AstParser;

public class ReaxWhileParse : INodeParser
{
    public bool IsParse(Token before, Token current, Token next)
    {
        return current.Type == TokenType.WHILE;
    }

    public ReaxNode? Parse(ITokenSource source)
    {
        source.Advance();
        var statement = source.NextStatement().ToArray();
        var condition = (BinaryNode)ExpressionHelper.Parser(statement);
        var block = (ContextNode)source.NextBlock();
        return new WhileNode(condition, block, condition.Location);
    }
}
