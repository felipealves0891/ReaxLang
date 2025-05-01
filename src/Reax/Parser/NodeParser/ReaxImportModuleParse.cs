using System;
using Reax.Debugger;
using Reax.Lexer;
using Reax.Parser.Node;
using Reax.Runtime.Contexts;
using Reax.Runtime.Functions;

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
        source.Advance();
        
        if(source.CurrentToken.Type != TokenType.END_STATEMENT)
            throw new InvalidOperationException("Era esperado o encerramento da expressão!");

        source.Advance();
        var functions = ReaxEnvironment.BuiltInRegistry.Get(identifier.Source);        
        var node = new ModuleNode(identifier.Source, functions, identifier.Location);
        DeclareFunctions(node);
        Logger.LogParse(node.ToString());
        return node;
    }

    private void DeclareFunctions(ModuleNode module)
    {
        ReaxEnvironment.AnalyzerContext.CreateModule(module.identifier);
        foreach (var keyValue in module.functions)
        {
            var name = keyValue.Key;
            var function = keyValue.Value;
            if(function is DecorateFunctionBuiltIn decorate)
            {
                var resultType = decorate.Result;
                var parameters = CreateParameters(name, decorate.RequiredParametersCount, decorate.Parameters);
                var symbol = new Symbol(name, resultType, SymbolCategory.FUNCTION, "main", new SourceLocation(), parameters);
                ReaxEnvironment.AnalyzerContext.Declare(symbol, module.identifier);
            }
        }
    }

    private IEnumerable<Symbol> CreateParameters(
        string name, 
        int requiredParameters,
        DataType[] types)
    {
        for (int i = 0; i < types.Length; i++)
        {
            var parameterName = $"{name}_parameter_{i}";
            var category = i < requiredParameters
                            ? SymbolCategory.PARAMETER
                            : SymbolCategory.PARAMETER_OPTIONAL;

            yield return new Symbol(
                    parameterName, 
                    types[i],
                    category,
                    name,
                    new SourceLocation());
        }

        
    }
}
