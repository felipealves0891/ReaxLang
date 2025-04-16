using System;
using Reax.Lexer;
using Reax.Lexer.Readers;
using Reax.Parser;

namespace Reax.Interpreter;

public class ReaxCompiler
{
    public static ReaxInterpreter Compile(string filename) 
    {
        var code = File.ReadAllText(filename);
        var lexer = new ReaxLexer(new ReaxTextReader(code));
        var tokens = lexer.Tokenize();

        var parser = new ReaxParser(tokens);
        var ast = parser.Parse();
        
        return new ReaxInterpreterBuilder()
                .AddFunctionsBuiltIn()
                .BuildMain(ast.ToArray());
    }
    
    public static ReaxInterpreter CompileModule(string module, string filename) 
    {
        var code = File.ReadAllText(filename);
        var lexer = new ReaxLexer(new ReaxTextReader(code));
        var tokens = lexer.Tokenize();

        var parser = new ReaxParser(tokens);
        var ast = parser.Parse();
        
        return new ReaxInterpreterBuilder(module)
                .AddFunctionsBuiltIn()
                .BuildModule(ast.ToArray());
    }
}
