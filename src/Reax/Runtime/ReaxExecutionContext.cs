using System;
using System.Collections.Concurrent;
using Reax.Interpreter;
using Reax.Parser.Node;
using Reax.Runtime.Functions;

namespace Reax.Runtime;

public class ReaxExecutionContext
{
    private readonly IDictionary<string, Guid> _variableKeys;
    private readonly IDictionary<Guid, ReaxNode> _variableContext;
    private readonly IDictionary<string, Guid> _functionKeys;
    private readonly IDictionary<Guid, Function> _functionContext;

    private readonly ReaxExecutionContext? _parentContext;
    

    public ReaxExecutionContext()
    {
        _variableKeys = new ConcurrentDictionary<string, Guid>();
        _variableContext = new ConcurrentDictionary<Guid, ReaxNode>();
        _functionKeys = new Dictionary<string, Guid>();
        _functionContext = new Dictionary<Guid, Function>();
    }

    public ReaxExecutionContext(ReaxExecutionContext parentContext)
        : this()
    {
        _parentContext = parentContext;
    }

    public ReaxExecutionContext GetParent() 
    {
        if(_parentContext is not null)
            return _parentContext;
        else 
            throw new InvalidOperationException("Não é possivel obter o contexto pai no contexto inicial");
    }

    public void DeclareVariable(string identifier)
    {
        _variableKeys[identifier] = Guid.NewGuid();
    }

    public void DeclareFunction(string identifier)
    {
        _functionKeys[identifier] = Guid.NewGuid();
    }

    public void SetVariable(string identifier, ReaxNode value)
    {
        if(!_variableKeys.TryGetValue(identifier, out var key))
            throw new InvalidOperationException($"Variavel '{identifier}' não declarada!");
        
        _variableContext[key] = value;
    }

    public void SetFunction(string identifier, ReaxInterpreter value)
    {
        if(!_functionKeys.TryGetValue(identifier, out var key))
            throw new InvalidOperationException($"Função '{identifier}' não declarada!");
        
        _functionContext[key] = new InterpreterFunction(identifier, value);
    }

    
    public void SetFunction(string identifier, Function value)
    {
        if(!_functionKeys.TryGetValue(identifier, out var key))
            throw new InvalidOperationException($"Função '{identifier}' não declarada!");
        
        _functionContext[key] = value;
    }

    public ReaxNode GetVariable(string identifier)
    {
        if(!_variableKeys.TryGetValue(identifier, out var key))
        {
            var valueContext = GetParentVariable(identifier);
            if(valueContext is not null)
                return valueContext;

            throw new InvalidOperationException($"Variavel '{identifier}' não declarada!");
        }
        
        if(!_variableContext.TryGetValue(key, out var value) || value is null)    
            throw new InvalidOperationException($"Variavel '{identifier}' não foi atribuida!");
        
        return value;
    }

    public Function GetFunction(string identifier)
    {
        if(!_functionKeys.TryGetValue(identifier, out var key))
        {
            var valueContext = GetParentFunction(identifier);
            if(valueContext is not null)
                return valueContext;

            throw new InvalidOperationException($"Variavel '{identifier}' não declarada!");
        }
        
        if(!_functionContext.TryGetValue(key, out var value) || value is null)    
            throw new InvalidOperationException($"Função '{identifier}' não foi atribuida!");
        
        return value;
    }

    private ReaxNode? GetParentVariable(string identifier) 
    {
        if(_parentContext is null)
            return null;
            
        if(!_parentContext._variableKeys.TryGetValue(identifier, out var key))
            return null;

        if(!_parentContext._variableContext.TryGetValue(key, out var value) || value is null)
            return null;

        return value;
    }
    
    private Function? GetParentFunction(string identifier) 
    {
        if(_parentContext is null)
            return null;
            
        if(!_parentContext._functionKeys.TryGetValue(identifier, out var key))
            return null;

        if(!_parentContext._functionContext.TryGetValue(key, out var value) || value is null)
            return null;

        return value;
    }
}
