using System;
using System.Diagnostics;
using Reax.Core;
using Reax.Core.Ast;
using Reax.Core.Debugger;
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
    public static IReaxInterpreter Compile(string filename) 
    {
        var ast = GetNodes(filename);   

        var analyser = new DefaultSemanticAnalyzer([
            new SymbolRule(),
            new ImmutableRule(),
            new TypeCheckingRule(),
            new CircularReferenceRule(),
            new ReturnFlowRule()
        ]);        

        var context = new SemanticContext();
        var results = ValidationResult.Success();
        
        foreach (var node in ast)
            results.Join(analyser.Analyze(node, context));
        
        results.Join(context.ValidateCycle());
        if(!results.Status)
        {
            Console.WriteLine(results.Message);
            Environment.Exit(-1);
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
