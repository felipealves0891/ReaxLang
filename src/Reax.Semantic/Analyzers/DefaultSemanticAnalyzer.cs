using System;
using Reax.Core.Debugger;
using Reax.Parser.Node;
using Reax.Core.Ast.Expressions;
using Reax.Core.Ast.Statements;
using Reax.Core.Ast;

namespace Reax.Semantic.Analyzers;

public class DefaultSemanticAnalyzer : ISemanticAnalyzer
{
    private readonly IEnumerable<ISemanticRule> _rules;
    private readonly ValidationResult _result;

    public DefaultSemanticAnalyzer(IEnumerable<ISemanticRule> rules)
    {
        _rules = rules;
        _result = ValidationResult.Success();
    }

    public ValidationResult Analyze(IReaxNode node, ISemanticContext context)
    {
        var disposables = new List<IDisposable?>([
            EnterScript(node, context)
        ]);
        
        Logger.LogAnalize($"({node.Location.Start.Line}) -> {node}");
        foreach (var rule in _rules)
        {
            _result.Join(rule.Apply(node, context));
            disposables.Add(rule.PrepareScope(context));
        }
        
        disposables.Add(EnterScope(node, context));

        foreach (var child in node.Children)
            _result.Join(Analyze(child, context));
    
        Dispose(disposables);
        disposables.Clear();
        
        return _result;
    }

    private IDisposable? EnterScript(IReaxNode node, ISemanticContext context)
    {
        if(node is ModuleNode module)
            return context.EnterScript(module.identifier);

        if(node is ScriptNode script)
        {
            var scriptDeclaration = node.Children.FirstOrDefault(x => x is ScriptDeclarationNode) as ScriptDeclarationNode;
            if(scriptDeclaration is null)
                return context.EnterScript(script.Identifier);
            
            script.Identifier = scriptDeclaration.Identifier;
            return context.EnterScript(script.Identifier);
        }
        
        return null;
    } 

    private IDisposable? EnterScope(IReaxNode node, ISemanticContext context)
    {
        if(node is StatementNode)
            return context.EnterScope();
        else 
            return null;
    }

    private void Dispose(IEnumerable<IDisposable?> disposables) 
    {
        foreach (var disposable in disposables)
            if(disposable is not null)
                disposable.Dispose();
    }
}

