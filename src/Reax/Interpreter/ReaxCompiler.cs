using System;
using System.Diagnostics;
using Reax.Debugger;
using Reax.Lexer;
using Reax.Lexer.Reader;
using Reax.Parser;
using Reax.Parser.Node;
using Reax.Semantic;
using Reax.Semantic.Analyzers;
using Reax.Semantic.Contexts;
using Reax.Semantic.Rules;

namespace Reax.Interpreter;

public class ReaxCompiler
{
    public static ReaxInterpreter Compile(string filename) 
    {
        var ast = GetNodes(filename);   

        var analyser = new DefaultSemanticAnalyzer([
            new SymbolRule()
        ]);        

        var context = new SemanticContext();
        var results = ast.Select(x => analyser.Analyze(x, context)).Aggregate(ValidationResult.Success(), (a, b) => a.Join(b));
        if(!results.Status)
        {
            throw new InvalidOperationException(results.Message);
        }

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
        var reader = new ReaxStreamReader(filename);
        var lexer = new ReaxLexer(reader);
        var tokens = lexer.Tokenize().ToArray();

        var parser = new ReaxParser(tokens);
        var ast = parser.Parse().ToArray();

        return ast;
    }
}
