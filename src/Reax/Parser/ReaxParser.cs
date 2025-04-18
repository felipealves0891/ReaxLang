using Reax.Lexer;
using Reax.Parser.Node;
using Reax.Parser.NodeParser;

namespace Reax.Parser;

public class ReaxParser : ITokenSource
{
    private readonly IList<INodeParser> _parsers;
    private readonly Token[] _tokens;
    private int _position;

    public ReaxParser(IEnumerable<Token> tokens)
    {
        _tokens = tokens.ToArray();
        _position = 0;
        _parsers = this.GetType()
                       .Assembly
                       .GetTypes()
                       .Where(x => x.GetInterface(nameof(INodeParser)) != null)
                       .Select(x => Activator.CreateInstance(x))
                       .Cast<INodeParser>()
                       .ToList();
    }

    public bool EndOfTokens => _position >= _tokens.Length;
    public Token BeforeToken => _position > 0 ? _tokens[_position-1] : new Token(TokenType.UNKNOW, (byte)' ', -1, -1);
    public Token CurrentToken => !EndOfTokens ? _tokens[_position] : new Token(TokenType.UNKNOW, (byte)' ', -1, -1);
    public Token NextToken => _position < _tokens.Length ? _tokens[_position+1] : new Token(TokenType.UNKNOW, (byte)' ', -1, -1);

    public IEnumerable<ReaxNode> Parse() 
    {
        ReaxNode? node = null;
        do 
        {
            node = NextNode();
            if(node is not null)
                yield return node;
        } 
        while(node is not null);
    }

    public ReaxNode? NextNode() 
    {
        if(_position >= _tokens.Length)
            return null;

        if(CurrentToken.Type == TokenType.EOF) 
            return null;

        foreach (var parser in _parsers)
        {
            if(parser.IsParse(BeforeToken, CurrentToken, NextToken))
            {
                return parser.Parse(this); 
            }
                
        }
        
        throw new Exception($"Token invalido na linha {CurrentToken.Row}: {CurrentToken}");
    }

    public IEnumerable<Token> NextStatement() 
    {
        while(true)
        {
            if(EndOfTokens)
                break;
            
            if(CurrentToken.Type == TokenType.START_BLOCK)                
                break;
        
            if(CurrentToken.Type == TokenType.END_STATEMENT) 
            {
                Advance();
                break;
            }
            
            yield return CurrentToken;
            Advance();
        }
    }

    public ReaxNode NextBlock()
    {
        var block = new List<ReaxNode>();
        if(CurrentToken.Type != TokenType.START_BLOCK)
            return new ContextNode(block.ToArray());

        Advance();
        while(CurrentToken.Type != TokenType.END_BLOCK)
        {
            var node = NextNode();
            if(node is null)
                throw new InvalidOperationException("Era esperado o fim do bloco!");

            block.Add(node);
        }

        Advance();
        return new ContextNode(block.ToArray());
    }

    public void Advance() 
    {
        if(_position + 1 > _tokens.Length)
            throw new InvalidOperationException($"Não é possivel avançar após o fim dos tokens: {BeforeToken} -> {CurrentToken}");

        _position++;
    }
}
