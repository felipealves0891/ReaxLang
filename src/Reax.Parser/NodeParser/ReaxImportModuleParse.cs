using System;
using Reax.Core.Debugger;
using Reax.Lexer;
using Reax.Parser.Node;
using Reax.Core.Ast.Statements;
using Reax.Core.Ast;
using Reax.Core.Registries;
using Reax.Core.Modules;

namespace Reax.Parser.NodeParser;

public class ReaxImportModuleParse : INodeParser
{
    public bool IsParse(Token before, Token current, Token next)
    {
        return current.Type == TokenType.IMPORT && next.Type == TokenType.MODULE; 
    }

    public ReaxNode? Parse(ITokenSource source)
    {
        source.Advance();
        source.Advance();
        var identifier = source.CurrentToken;
        source.Advance(TokenType.SEMICOLON);
        
        source.Advance();
        return ModuleResolver.GetInstance()
            .Resolve(identifier.Source, identifier.Location);
    }
}
