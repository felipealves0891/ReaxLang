using System;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;

namespace Reax.Core.Registries;

public abstract class BaseRegistry<Tkey, TData>
    where Tkey : notnull
{
    private static readonly ConcurrentDictionary<Tkey, TData> _registry = new();

    public BaseRegistry()
    {}
    
    public IReadOnlyDictionary<Tkey, TData> Registry => new ReadOnlyDictionary<Tkey, TData>(_registry);

    public virtual TData this[Tkey key]
    {
        get => Get(key);
        set
        {
            if(Exists(key))
                Update(key, value);
            else
                Set(key, value);
        }
    }

    public virtual bool Set(Tkey key, TData data) 
    {
        return _registry.TryAdd(key, data);
    }
    
    public virtual bool Update(Tkey key, TData data) 
    {
        try
        {
            _registry[key] = data;
            return true;    
        }
        catch (System.Exception)
        {
            return false;
        }
        
    }

    public virtual bool Exists(Tkey key) 
    {
        return _registry.ContainsKey(key);
    }

    public virtual TData Get(Tkey key) 
    {
        if(_registry.TryGetValue(key, out var data))
            return data;

        var newData = Load(key);
        if(newData is null)
            throw new InvalidOperationException($"Não foi possivel localizar um item para a chave {key}!");
        
        Set(key, newData);
        return newData;
    }

    protected abstract TData? Load(Tkey key);
}
