using System;

namespace Reax.Semantic.Scopes;

public class ReferenceVisitor
{
    private readonly HashSet<string> _visited = new();
    private readonly HashSet<string> _stack = new();
    private readonly Dictionary<string, IList<string>> _dependencies = new();

    public void AddDependency(string from, string to)
    {
        if(!_dependencies.ContainsKey(from)) _dependencies[from] = new List<string>();
        _dependencies[from].Add(to);
    }

    public bool HasDependencyCycle()
    {
        return _dependencies.Keys.Any(Visit);
    }

    public string GetPathDependencyCycle()
    {
        return string.Empty;
    }

    private bool Visit(string node) 
    {
        if(_stack.Contains(node)) return true;
        if(_visited.Contains(node)) return false;

        _visited.Add(node);
        _stack.Add(node);

        if(_dependencies.TryGetValue(node, out var targets))
            foreach (var neighbor in targets)
                if(Visit(neighbor)) return true;

        _stack.Remove(node);
        return false;
    }
}
