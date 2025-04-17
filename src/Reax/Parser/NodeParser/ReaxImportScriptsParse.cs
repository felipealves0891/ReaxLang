using System;
using Reax.Interpreter;
using Reax.Lexer;
using Reax.Parser.Node;

namespace Reax.Parser.NodeParser;

public class ReaxImportScriptsParse : INodeParser
{
    public bool IsParse(Token before, Token current, Token next)
    {
        return current.Type == TokenType.IMPORT;
    }

    public ReaxNode? Parse(ITokenSource source)
    {
        source.Advance();
        var file = source.CurrentToken.ReadOnlySource.ToString();
        var info = new FileInfo(Path.Combine(ReaxEnvironment.DirectoryRoot, file));
        
        if(!info.Exists) throw new InvalidOperationException($"Modulo '{file}' não localizado!");
        ModuleNode? module = null;

        if(!ReaxEnvironment.ImportedFiles.ContainsKey(file))
        {
            var interpreter = ReaxCompiler.CompileModule(file, info.FullName);
            var moduleName = file.Replace(".reax", "").Replace("\\", ".").Replace("/", ".");
            module = new ModuleNode(moduleName, interpreter);
            ReaxEnvironment.ImportedFiles.Add(file, module);      
        }
        else
        {
            module = ReaxEnvironment.ImportedFiles[file];
        }

        source.Advance();
        if(source.CurrentToken.Type != TokenType.END_STATEMENT)
            throw new InvalidOperationException($"Era esperado o fim da expressão na linha {source.CurrentToken.Row}!");

        source.Advance();
        if(module is null)
            throw new InvalidOperationException($"ERRO: modulo não foi importado!");

        return module;
    }
}
