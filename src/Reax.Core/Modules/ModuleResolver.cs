using System;
using Reax.Core.Ast;
using Reax.Core.Ast.Statements;
using Reax.Core.Locations;
using Reax.Core.Registries;
using Reax.Parser.Node;

namespace Reax.Core.Modules;

public class ModuleResolver
{
    private readonly FunctionBuiltInRegistry _functions;
    private ModuleResolver()
    {
        _functions = new FunctionBuiltInRegistry();
    }

    public ReaxNode Resolve(string identifier, SourceLocation location)
    {
        if (_functions.Exists(identifier))
            return new NullNode(location);

        var functions = _functions.Get(identifier);
        return new ModuleNode(identifier, functions, location);
    }

    private static ModuleResolver? _instance;
    public static ModuleResolver GetInstance()
    {
        if (_instance is null)
            _instance = new ModuleResolver();

        return _instance;
    }
}
