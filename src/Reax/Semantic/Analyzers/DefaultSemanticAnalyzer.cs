using System;

namespace Reax.Semantic.Analyzers;

public class DefaultSemanticAnalyzer : ISemanticAnalyzer
{
    private readonly List<ISemanticRule> _rules;
    private readonly ValidationResult _result;

    public DefaultSemanticAnalyzer(IEnumerable<ISemanticRule> rules)
    {
        _rules = rules.ToList();
        _result = ValidationResult.Success();
    }

    public ValidationResult Analyze(IReaxNode node, ISemanticContext context)
    {
        foreach (var rule in _rules)
            _result.Join(rule.Apply(node, context));

        foreach (var child in node.Children)
            _result.Join(Analyze(child, context));
        
        return _result;
    }
}

