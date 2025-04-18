using System;
using Reax.Interpreter;
using Reax.Lexer;
using Reax.Parser.Node;

namespace Reax.Parser.NodeParser;

public class ReaxImportScriptsParse : INodeParser
{
    public bool IsParse(Token before, Token current, Token next)
    {
        return current.Type == TokenType.IMPORT && next.Type == TokenType.SCRIPT;
    }

    public ReaxNode? Parse(ITokenSource source)
    {
        source.Advance();
        source.Advance();
        var file = source.CurrentToken.ReadOnlySource.ToString();
        var info = new FileInfo(Path.Combine(ReaxEnvironment.DirectoryRoot, file));
        
        if(!info.Exists) throw new InvalidOperationException($"Modulo '{file}' não localizado!");
        ScriptNode? script = null;

        if(!ReaxEnvironment.ImportedFiles.ContainsKey(file))
        {
            var interpreter = ReaxCompiler.CompileScript(file, info.FullName);
            var scriptName = file.Replace(".reax", "").Replace("\\", ".").Replace("/", ".");
            script = new ScriptNode(scriptName, interpreter);
            ReaxEnvironment.ImportedFiles.Add(file, script);      
        }
        else
        {
            script = ReaxEnvironment.ImportedFiles[file];
        }

        source.Advance();
        if(source.CurrentToken.Type != TokenType.END_STATEMENT)
            throw new InvalidOperationException($"Era esperado o fim da expressão na linha {source.CurrentToken.Row}!");

        source.Advance();
        if(script is null)
            throw new InvalidOperationException($"ERRO: modulo não foi importado!");

        return script;
    }
}
