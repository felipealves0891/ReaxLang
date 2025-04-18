using System.Collections.Concurrent;
using Reax.Interpreter;
using Reax.Parser.Node;
using Reax.Runtime.Functions;
using Reax.Runtime.Observables;

namespace Reax.Runtime;

public class ReaxExecutionContext
{
    private readonly ISet<Guid> _immutableKeys;
    private readonly ISet<Guid> _asyncKeys;
    private readonly IDictionary<string, Guid> _variableKeys;
    private readonly IDictionary<string, Guid> _functionKeys;
    private readonly IDictionary<string, Guid> _scriptKeys;
    private readonly IDictionary<string, Guid> _moduleKeys;
    private readonly IDictionary<Guid, ReaxNode> _variableContext;
    private readonly IDictionary<Guid, Function> _functionContext;
    private readonly IDictionary<Guid, ReaxInterpreter> _scriptContext;
    private readonly IDictionary<Guid, Dictionary<string, Function>> _moduleContext;
    private readonly IDictionary<Guid, IList<VariableObservable>> _observableContext;
    private readonly ReaxExecutionContext? _parentContext;
    private readonly string _name;

    public ReaxExecutionContext(string name)
    {
        _variableKeys = new ConcurrentDictionary<string, Guid>();
        _variableContext = new ConcurrentDictionary<Guid, ReaxNode>();
        _functionKeys = new Dictionary<string, Guid>();
        _functionContext = new Dictionary<Guid, Function>();
        _moduleKeys = new Dictionary<string, Guid>();
        _observableContext = new Dictionary<Guid, IList<VariableObservable>>();
        _scriptKeys = new Dictionary<string, Guid>();
        _scriptContext = new Dictionary<Guid, ReaxInterpreter>();
        _moduleContext = new Dictionary<Guid, Dictionary<string, Function>>();
        _name = name;
        _immutableKeys = new HashSet<Guid>();
        _asyncKeys = new HashSet<Guid>();
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

    public bool ScriptExists(string identifier) 
    {
        return _scriptKeys.ContainsKey(identifier) 
           || (_parentContext is not null && _parentContext.ScriptExists(identifier));
    }

    public bool ModuleExists(string identifier) 
    {
        return _moduleKeys.ContainsKey(identifier)
           || (_parentContext is not null && _parentContext.ModuleExists(identifier));
    }

    public void DeclareVariable(string identifier, bool isAsync)
    {
        var key = Guid.NewGuid();
        _variableKeys[identifier] = key;
        if(isAsync) _asyncKeys.Add(key);
    }
    
    public void DeclareImmutable(string identifier, ReaxNode node)
    {
        var key = Guid.NewGuid();
        _variableKeys[identifier] = key;
        _immutableKeys.Add(key);
        _variableContext[key] = node;
    }

    public void DeclareFunction(string identifier)
    {
        _functionKeys[identifier] = Guid.NewGuid();
    }

    public void DeclareScript(string identifier)
    {
        _scriptKeys[identifier] = Guid.NewGuid();
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

        if(_immutableKeys.Contains(key))
            throw new InvalidOperationException($"A variavel {identifier} é imutavel, não pode ser reatribuida!");

        _variableContext[key] = value;
        OnChange(key);
    }

    private void OnChange(Guid key) 
    {
        if(!_observableContext.TryGetValue(key, out var observables))
            return;

        
        if(_asyncKeys.Contains(key))
        {
            Parallel.ForEach(observables, observable => {
                if(observable.CanRun(this)) observable.Run();
            });
        }
        else
        {
            foreach (var observable in observables)
                if(observable.CanRun(this)) observable.Run();
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
        {
            if(_immutableKeys.Contains(local))
                throw new InvalidOperationException("Não é possivel observar um constante!");

            SetObservable(local, observable);
        }
        else if(_parentContext is null)
            throw new InvalidOperationException($"{_name}: Não é possivel observar uma variavel não declarada: variavel '{identifier}'!");
        else if(_parentContext._variableKeys.TryGetValue(identifier, out var parent))
            _parentContext.SetObservable(parent, observable);
    }

    private void SetObservable(Guid key, VariableObservable observable)
    {
        if(_observableContext.TryGetValue(key, out var observables))
            observables.Add(observable);
        else 
            _observableContext[key] = new List<VariableObservable>([observable]);
    }
    
    public void SetScript(string identifier, ReaxInterpreter interpreter)
    {
        if(!_scriptKeys.TryGetValue(identifier, out var key))
            throw new InvalidOperationException($"{_name}: O script {identifier} não foi importado!");
        
        _scriptContext[key] = interpreter;
    } 
    
    public void SetModule(string identifier, Dictionary<string, Function> functions)
    {
        if(!_moduleKeys.TryGetValue(identifier, out var key))
            throw new InvalidOperationException($"{_name}: O modulo {identifier} não foi importado!");
        
        _moduleContext[key] = functions;
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

    public ReaxInterpreter GetScript(string identifier)
    {
        if(!_scriptKeys.TryGetValue(identifier, out var key))
        {
            var script = GetParentScript(identifier);
            if(script is not null)
                return script;

            throw new InvalidOperationException($"{_name}: O script {identifier} não foi declarado");
        }

        if(!_scriptContext.TryGetValue(key, out var interpreter))
            throw new InvalidOperationException($"{_name}: O script {identifier} não foi definido");

        return interpreter;
    }

    private ReaxInterpreter? GetParentScript(string identifier) 
    {
        try
        {
            if(_parentContext is null)
                return null;

            return _parentContext.GetScript(identifier);
        }
        catch (System.Exception)
        {
            return null;
        }
    }
    
    public Function GetModule(string identifier, string functionName)
    {
        if(!_moduleKeys.TryGetValue(identifier, out var key))
        {
            var module = GetParentModule(identifier, functionName);
            if(module is not null)
                return module;

            throw new InvalidOperationException($"{_name}: O modulo {identifier} não foi localizado");
        }

        if(!_moduleContext.TryGetValue(key, out var functions))
            throw new InvalidOperationException($"{_name}: O modulo {identifier} não foi localizado");

        if(functions.TryGetValue(functionName, out var function))
            return function;

        throw new InvalidOperationException($"{_name}: A função {functionName} do modulo {identifier} não foi localizado");
        
    }

    public Function? GetParentModule(string identifier, string functionName)
    {
        try
        {
            if(_parentContext is null)
                return null;

            return _parentContext.GetModule(identifier, functionName);
        }
        catch (System.Exception)
        {
            return null;
        }
    }

}
