using System;
using Reax.Lexer;
using Reax.Parser.Node;

namespace Reax.Parser.Helper;

public class ComparisonHelper
{
    private readonly Token[] _tokens;
    private int _pos;

    private bool EndOfTokens => _pos >= _tokens.Length;
    private Token CurrentToken => _tokens[_pos];

    public ComparisonHelper(IEnumerable<Token> tokens)
    {
        _tokens = tokens.ToArray();
        _pos = 0;
    }

    public BinaryNode Parse() 
    {
        var left = GetBinaryNode();
        while(!EndOfTokens)
        {
            if(CurrentToken.Type == TokenType.END_PARAMETER)
            {
                Advance();
                return left;
            }
            
            var operation = CurrentToken;
            Advance();
            var right = GetBinaryNode();
            left = new BinaryNode(left, operation.ToLogicOperator(), right, operation.Location);
        }
        return left;
    } 

    private BinaryNode GetBinaryNode() 
    {
        if(!CurrentToken.IsReaxValue() && CurrentToken.Type == TokenType.START_PARAMETER)
        {
            Advance();
            var node = Parse();            
            return node;
        }
        
        var left = CurrentToken;
        Advance();
        var operation = CurrentToken;
        Advance();
        var right = CurrentToken;
        Advance();

        return new BinaryNode(
            left.ToReaxValue(), 
            operation.ToLogicOperator(), 
            right.ToReaxValue(), 
            operation.Location);
    }

    private void Advance() 
    {
        _pos++;
    }
}
