using System;
using Reax.Runtime.Functions;

namespace Reax.Parser.Node;

public record ModuleNode(string identifier, Dictionary<string, Function> functions) : ReaxNode 
{
    public override string ToString()
    {
        return $"import module {identifier};";
    }
}
