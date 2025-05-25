using System;
using System.Diagnostics;
using Reax.Core;
using Reax.Core.Ast;
using Reax.Core.Debugger;
using Reax.Interpreter.Cache;
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
    private static bool _reprocessing = false;

    public static IReaxInterpreter Compile(string filename)
    {
        var ast = GetNodes(filename);
        return new ReaxInterpreterBuilder()
                .BuildMain(ast);
    }

    private static ReaxNode[] GetNodes(string filename)
    {
        var ast = ReaxBinSerializer.TryLoadAstIfHashMatches(filename);
        if (ast is not null && !_reprocessing)
        {
            Debug.WriteLine("Using cached AST for: " + filename);
            return ast;
        }

        var reader = new ReaxStreamReader(filename);
        var lexer = new ReaxLexer(reader);
        var tokens = lexer.Tokenize().ToArray();

        var parser = new ReaxParser(tokens, GetNodes);
        ast = parser.Parse().ToArray();
        
        RunAnalyzeSemantic(ast);    
        ReaxBinSerializer.SerializeAstToBinary(filename, ast);
        
        return ast;
    }

    private static void RunAnalyzeSemantic(IEnumerable<ReaxNode> ast)
    {
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
        if (!results.Status)
        {
            Console.WriteLine(results.Message);
            Environment.Exit(-1);
        }
    }
}
