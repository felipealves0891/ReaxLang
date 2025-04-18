using System;
using Reax.Debugger;
using Reax.Lexer;
using Reax.Parser.Helper;
using Reax.Parser.Node;

namespace Reax.Parser.NodeParser;

public class ReaxArithmeticOperationParse : INodeParser
{
    public bool IsParse(Token before, Token current, Token next)
    {
        return current.CanCalculated() && next.IsArithmeticOperator();
    }

    public ReaxNode? Parse(ITokenSource source)
    {
        var statement = source.NextStatement();
        var helper = new CalculationHelper(statement);
        var node = helper.ParseExpression();
        
        if(node is null)
            throw new InvalidOperationException("Valores faltando para a operação");

        Logger.LogParse(node.ToString());
        return node;
    }
}
