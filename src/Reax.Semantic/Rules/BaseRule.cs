using System;
using Reax.Core.Ast;
using Reax.Semantic.Contexts;

namespace Reax.Semantic.Rules;

public abstract class BaseRule : ISemanticRule
{
    protected ISemanticContext Context;
    protected IDictionary<Type, Func<IReaxNode, ValidationResult>> Handlers;

    public BaseRule()
    {
        Handlers = new Dictionary<Type, Func<IReaxNode, ValidationResult>>();
        Context = new SemanticContext();
    }

    public ValidationResult Apply(IReaxNode node, ISemanticContext context)
    {
        Context = context;
        var type = node.GetType();

        PrepareApply();
        if(Handlers.TryGetValue(type, out var handler))
            return handler(node);

        return ValidationResult.Success();
    }

    protected virtual void PrepareApply()
    {}

    public virtual IDisposable? PrepareScope(ISemanticContext context)
        => null!;
}
