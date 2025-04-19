using System;
using Reax.Debugger;
using Reax.Lexer;
using Reax.Parser.Node;
using Reax.Runtime;
using Reax.Runtime.Symbols;

namespace Reax.Parser.NodeParser;

public class ReaxDeclarationParse : INodeParser
{
    public bool IsParse(Token before, Token current, Token next)
    {
        return current.Type == TokenType.LET || current.Type == TokenType.CONST
           || (current.Type == TokenType.ASYNC && next.Type == TokenType.LET);
    }

    public ReaxNode? Parse(ITokenSource source)
    {
        Token? identifier = null;
        Token? value = null;
        Token? dataType = null;
        
        var isAsync = source.CurrentToken.Type == TokenType.ASYNC;
        if(isAsync) source.Advance();

        var immutable = source.CurrentToken.Type == TokenType.CONST;
        var isTyping = false;

        foreach (var statement in source.NextStatement())
        {
            if(statement.Type == TokenType.IDENTIFIER && !identifier.HasValue)
                identifier = statement;
            else if(statement.Type == TokenType.TYPING)
                isTyping = true;
            else if(isTyping && !dataType.HasValue)
                dataType = statement;    
            else if (statement.IsReaxValue())
                value = statement;
        }

        if(immutable && value is null)
            throw new InvalidOperationException("A constante deve ser definida na declaração!");

        if(identifier is null)
            throw new InvalidOperationException("Era esperado um identificar!");

        if(dataType is null)
            throw new InvalidOperationException("Era esperado o tipo da variavel!");

        var textIdentifier = identifier.Value.Source;
        ReaxNode node;
        if(value is not null)
            node = new DeclarationNode(textIdentifier, immutable, isAsync, value.Value.ToReaxValue(), identifier.Value.Location);
        else 
            node = new DeclarationNode(textIdentifier, immutable,  isAsync, null, identifier.Value.Location);

        ReaxEnvironment.Symbols.UpdateSymbol(
            textIdentifier, 
            dataType.Value.Source, 
            immutable, 
            isAsync,
            immutable ? SymbolCategoty.CONST : SymbolCategoty.LET);
            
        Logger.LogParse(node.ToString());
        return node;
    }
}
