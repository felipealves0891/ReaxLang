using System;

namespace Reax.Runtime.Contexts.Models;

public class Scope : Dictionary<string, Symbol>
{
    public string Name { get; init; }
    public Scope(string name)
    {
        Name = name;
    }
}
