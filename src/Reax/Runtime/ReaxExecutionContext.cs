using System;
using Reax.Parser.Node;

namespace Reax.Runtime;

public class ReaxExecutionContext
{
    private readonly Dictionary<string, Guid> _keys;
    private readonly Dictionary<Guid, ReaxNode> _context;
    private readonly ReaxExecutionContext? _parentContext;

    public ReaxExecutionContext()
    {
        _keys = new Dictionary<string, Guid>();
        _context = new Dictionary<Guid, ReaxNode>();
    }

    public ReaxExecutionContext(ReaxExecutionContext parentContext)
    {
        _keys = new Dictionary<string, Guid>();
        _context = new Dictionary<Guid, ReaxNode>();
        _parentContext = parentContext;
    }

    public ReaxExecutionContext GetParent() 
    {
        if(_parentContext is not null)
            return _parentContext;
        else 
            throw new InvalidOperationException("Não é possivel obter o contexto pai no contexto inicial");
    }

    public void Declare(string identifier)
    {
        _keys[identifier] = Guid.NewGuid();
    }

    public void SetValue(string identifier, ReaxNode value)
    {
        if(!_keys.TryGetValue(identifier, out var key))
            throw new InvalidOperationException($"Variavel '{identifier}' não declarada!");
        
        _context[key] = value;
    }

    public ReaxNode GetValue(string identifier)
    {
        if(!_keys.TryGetValue(identifier, out var key))
        {
            var valueContext = GetParentValue(identifier);
            if(valueContext is not null)
                return valueContext;

            throw new InvalidOperationException($"Variavel '{identifier}' não declarada!");
        }
        
        if(!_context.TryGetValue(key, out var value) || value is null)    
            throw new InvalidOperationException($"Variavel '{identifier}' não foi atribuida!");
        
        return value;
    }

    private ReaxNode? GetParentValue(string identifier) 
    {
        if(_parentContext is null)
            return null;
            
        if(!_parentContext._keys.TryGetValue(identifier, out var key))
            return null;

        if(!_parentContext._context.TryGetValue(key, out var value) || value is null)
            return null;

        return value;
    }
}
