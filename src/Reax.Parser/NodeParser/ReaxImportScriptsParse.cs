using System;
using Reax.Core;
using Reax.Core.Ast;
using Reax.Core.Ast.Statements;
using Reax.Core.Debugger;
using Reax.Lexer;
using Reax.Lexer.Reader;
using Reax.Parser.Node;

namespace Reax.Parser.NodeParser;

public class ReaxImportScriptsParse : INodeParser
{
    private readonly Func<string, ReaxNode[]> _getNodes;

    public ReaxImportScriptsParse(Func<string, ReaxNode[]> getNodes)
    {
        _getNodes = getNodes;
    }

    public bool IsParse(Token before, Token current, Token next)
    {
        return current.Type == TokenType.IMPORT && next.Type == TokenType.SCRIPT;
    }

    public ReaxNode? Parse(ITokenSource source)
    {
        source.Advance();
        source.Advance();
        var file = source.CurrentToken.Source;
        var info = new FileInfo(Path.Combine(ReaxEnvironment.DirectoryRoot, file));
        
        if(!info.Exists) throw new InvalidOperationException($"Modulo '{file}' não localizado!");
        ScriptNode? script = null;

        if(!ReaxEnvironment.ImportedFiles.ContainsKey(file))
        {
            var nodes = _getNodes(info.FullName);
            script = new ScriptNode(info.Name, nodes.ToArray(), source.CurrentToken.Location);
            ReaxEnvironment.ImportedFiles.Add(file, script);      
        }
        else
        {
            script = ReaxEnvironment.ImportedFiles[file];
        }

        source.Advance();
        if(source.CurrentToken.Type != TokenType.SEMICOLON)
            throw new InvalidOperationException($"Era esperado o fim da expressão na linha {source.CurrentToken.Row}!");

        source.Advance();
        if(script is null)
            throw new InvalidOperationException($"ERRO: modulo não foi importado!");

        return script;
    }
    
}
