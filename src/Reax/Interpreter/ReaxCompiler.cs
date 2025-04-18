using System;
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
                .AddFunctionsBuiltIn()
                .BuildMain(ast.ToArray());
    }
    
    public static ReaxInterpreter CompileScript(string script, string filename) 
    {
        var ast = GetNodes(filename);
        return new ReaxInterpreterBuilder(script)
                .AddFunctionsBuiltIn()
                .BuildScript(ast.ToArray());
    }

    private static IEnumerable<ReaxNode> GetNodes(string filename)
    {
        var reader = new ReaxStreamReader(filename);
        var lexer = new ReaxLexer(reader);
        var tokens = lexer.Tokenize();

        var parser = new ReaxParser(tokens);
        return parser.Parse();
    }
}
