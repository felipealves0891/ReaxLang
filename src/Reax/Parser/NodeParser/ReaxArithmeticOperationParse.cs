using System;
using Reax.Core.Debugger;
using Reax.Lexer;
using Reax.Parser.Extensions;
using Reax.Parser.Helper;
using Reax.Parser.Node;

namespace Reax.Parser.NodeParser;

public class ReaxArithmeticOperationParse : INodeParser
{
    public bool IsParse(Token before, Token current, Token next)
    {
        return (current.CanCalculated() && next.IsArithmeticOperator()) ||
               (current.Type == TokenType.START_PARAMETER && next.CanCalculated());
    }

    public ReaxNode? Parse(ITokenSource source)
    {
        var statement = source.NextStatement();
        var node = ExpressionHelper.Parser(statement.ToArray());
        
        if(node is null)
            throw new InvalidOperationException("Valores faltando para a operação");

        Logger.LogParse(node.ToString());
        return node;
    }
}
