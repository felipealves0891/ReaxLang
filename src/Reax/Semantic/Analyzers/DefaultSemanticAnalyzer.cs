using System;
using Reax.Debugger;
using Reax.Parser.Node;
using Reax.Parser.Node.Expressions;
using Reax.Parser.Node.Statements;

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
        
        Logger.LogAnalize($"({node.Location.Line}) -> {node}");
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
        if(node is ScriptNode script)
            return context.EnterScript(script.Identifier);
        else if(node is ModuleNode module)
            return context.EnterScript(module.identifier);
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

