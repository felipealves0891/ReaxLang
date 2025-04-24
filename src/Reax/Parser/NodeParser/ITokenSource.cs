using System;
using Reax.Lexer;
using Reax.Parser.Node;

namespace Reax.Parser.NodeParser;

public interface ITokenSource
{
    public bool EndOfTokens { get; }
    public Token BeforeToken { get; }
    public Token CurrentToken { get; }
    public Token NextToken { get; }
    public ReaxNode? NextNode();
    IEnumerable<Token> NextStatement();
    ReaxNode NextBlock();
    void Advance();
    void Advance(TokenType expectedType);
}
