using System;
using System.Diagnostics;
using Reax.Debugger;
using Reax.Lexer;
using Reax.Lexer.Reader;
using Reax.Parser;
using Reax.Parser.Node;

namespace Reax.Interpreter;

public class ReaxCompiler
{
    public static ReaxInterpreter Compile(string filename) 
    {
        var ast = GetNodes(filename);   
        return new ReaxInterpreterBuilder()
                .BuildMain(ast.ToArray());
    }
    
    public static ReaxInterpreter CompileScript(string script, string filename) 
    {
        var ast = GetNodes(filename);
        return new ReaxInterpreterBuilder(script)
                .BuildScript(ast.ToArray());
    }

    private static IEnumerable<ReaxNode> GetNodes(string filename)
    {
        var analizer = new Analyzer();        
        analizer.Start();
        var reader = new ReaxStreamReader(filename);
        var lexer = new ReaxLexer(reader);
        var tokens = lexer.Tokenize().ToArray();
        Logger.LogCompile(analizer.Stop());

        analizer.Start();
        var parser = new ReaxParser(tokens);
        var ast = parser.Parse();
        Logger.LogCompile(analizer.Stop());

        return ast;
    }
}
