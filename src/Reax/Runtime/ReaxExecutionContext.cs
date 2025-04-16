using System;
using System.Collections.Concurrent;
using Reax.Interpreter;
using Reax.Parser.Node;
using Reax.Runtime.Functions;
using Reax.Runtime.Observables;

namespace Reax.Runtime;

public class ReaxExecutionContext
{
    private readonly IDictionary<string, Guid> _variableKeys;
    private readonly IDictionary<string, Guid> _functionKeys;
    private readonly IDictionary<string, Guid> _moduleKeys;
    private readonly IDictionary<Guid, ReaxNode> _variableContext;
    private readonly IDictionary<Guid, Function> _functionContext;
    private readonly IDictionary<Guid, ReaxInterpreter> _moduleContext;
    private readonly IDictionary<Guid, IList<VariableObservable>> _observableContext;
    private readonly ReaxExecutionContext? _parentContext;
    private readonly string _name;

    public ReaxExecutionContext(string name)
    {
        _variableKeys = new ConcurrentDictionary<string, Guid>();
        _variableContext = new ConcurrentDictionary<Guid, ReaxNode>();
        _functionKeys = new Dictionary<string, Guid>();
        _functionContext = new Dictionary<Guid, Function>();
        _observableContext = new Dictionary<Guid, IList<VariableObservable>>();
        _moduleKeys = new Dictionary<string, Guid>();
        _moduleContext = new Dictionary<Guid, ReaxInterpreter>();
        _name = name;
    }

    public ReaxExecutionContext(string name, ReaxExecutionContext parentContext)
        : this($"{parentContext._name}->{name}")
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

    public void DeclareModule(string identifier)
    {
        _moduleKeys[identifier] = Guid.NewGuid();
    }

    public void SetVariable(string identifier, ReaxNode value)
    {
        if(!_variableKeys.TryGetValue(identifier, out var key))
        {
            if(_parentContext is not null && _parentContext._variableKeys.TryGetValue(identifier, out var parentKey))
            {
                _parentContext._variableContext[parentKey] = value;
                _parentContext.OnChange(parentKey);
                return;
            }

            throw new InvalidOperationException($"{_name}: Variavel '{identifier}' não declarada!");
        }

        _variableContext[key] = value;
        OnChange(key);
    }

    private void OnChange(Guid key) 
    {
        if(_observableContext.TryGetValue(key, out var observables))
        {
            Parallel.ForEach(observables, observable => {
                if(observable.CanRun(this)) observable.Run();
            });
        }
    }

    public void SetFunction(string identifier, ReaxInterpreter value)
    {
        if(!_functionKeys.TryGetValue(identifier, out var key))
            throw new InvalidOperationException($"{_name}: Função '{identifier}' não declarada!");
        
        _functionContext[key] = new InterpreterFunction(identifier, value);
    }

    public void SetFunction(string identifier, Function value)
    {
        if(!_functionKeys.TryGetValue(identifier, out var key))
            throw new InvalidOperationException($"{_name}: Função '{identifier}' não declarada!");
        
        _functionContext[key] = value;
    }

    public void SetObservable(string identifier, ReaxInterpreter interpreter, BinaryNode? condition)
    {
        var observable = new VariableObservable(interpreter, condition);
        if(_variableKeys.TryGetValue(identifier, out var local))
            SetObservable(local, observable);
        else if(_parentContext is null)
            throw new InvalidOperationException($"{_name}: Não é possivel observar uma variavel não declarada: variavel '{identifier}'!");
        else if(_parentContext._variableKeys.TryGetValue(identifier, out var parent))
            SetObservable(parent, observable);
    }

    private void SetObservable(Guid key, VariableObservable observable)
    {
        if(_observableContext.TryGetValue(key, out var observables))
            observables.Add(observable);
        else 
            _observableContext[key] = new List<VariableObservable>([observable]);
    }
    
    public void SetModule(string identifier, ReaxInterpreter interpreter)
    {
        if(!_moduleKeys.TryGetValue(identifier, out var key))
            throw new InvalidOperationException($"{_name}: O modulo {identifier} não foi importado!");
        
        _moduleContext[key] = interpreter;
    } 

    public ReaxNode GetVariable(string identifier)
    {
        if(!_variableKeys.TryGetValue(identifier, out var key))
        {
            var valueContext = GetParentVariable(identifier);
            if(valueContext is not null)
                return valueContext;

            throw new InvalidOperationException($"{_name}: Variavel '{identifier}' não declarada!");
        }
        
        if(!_variableContext.TryGetValue(key, out var value) || value is null)    
            throw new InvalidOperationException($"{_name}: Variavel '{identifier}' não foi atribuida!");
        
        return value;
    }

    public Function GetFunction(string identifier)
    {
        if(!_functionKeys.TryGetValue(identifier, out var key))
        {
            var valueContext = GetParentFunction(identifier);
            if(valueContext is not null)
                return valueContext;

            throw new InvalidOperationException($"{_name}: Função '{identifier}' não declarada!");
        }   
        
        if(!_functionContext.TryGetValue(key, out var value) || value is null)    
            throw new InvalidOperationException($"{_name}: Função '{identifier}' não foi atribuida!");
        
        return value;
    }

    private ReaxNode? GetParentVariable(string identifier) 
    {
        try
        {
            return _parentContext?.GetVariable(identifier);
        }
        catch (System.Exception)
        {
            return null;
        }
    }
    
    private Function? GetParentFunction(string identifier) 
    {
        try
        {
            return _parentContext?.GetFunction(identifier);
        }
        catch (System.Exception)
        {
            return null;
        }
    }

    public ReaxInterpreter GetModule(string identifier)
    {
        if(!_moduleKeys.TryGetValue(identifier, out var key))
        {
            var module = GetParentModule(identifier);
            if(module is not null)
                return module;

            throw new InvalidOperationException($"{_name}: O modulo {identifier} não foi declarado");
        }

        if(!_moduleContext.TryGetValue(key, out var interpreter))
            throw new InvalidOperationException($"{_name}: O modulo {identifier} não foi definido");

        return interpreter;
    }

    private ReaxInterpreter? GetParentModule(string identifier) 
    {
        try
        {
            if(_parentContext is null)
                return null;

            return _parentContext.GetModule(identifier);
        }
        catch (System.Exception)
        {
            return null;
        }
    }

}
