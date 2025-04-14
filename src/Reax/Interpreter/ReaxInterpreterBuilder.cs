using System;
using System.Reflection;
using Reax.Parser.Node;
using Reax.Runtime.Functions;
using Reax.Runtime.Functions.Attributes;

namespace Reax.Interpreter;

public class ReaxInterpreterBuilder
{
    private static ReaxInterpreter? _instanceMain = null; 

    public IEnumerable<(string identifier, Function function)> FunctionsBuiltIn { get; set; }

    public ReaxInterpreterBuilder()
    {
        FunctionsBuiltIn = Enumerable.Empty<(string identifier, Function function)>();
    }

    public ReaxInterpreterBuilder AddFunctionsBuiltIn() 
    {
        FunctionsBuiltIn = this.GetType()
                               .Assembly
                               .GetTypes()
                               .Where(x => x.GetCustomAttribute<FunctionBuiltInAttribute>() is not null)
                               .Select(x => 
                               {
                                   var attribute = x.GetCustomAttribute<FunctionBuiltInAttribute>() ?? throw new InvalidOperationException();
                                   var function = (Activator.CreateInstance(x) as Function) ?? throw new InvalidOperationException();
                                   var decorator = new DecorateFunctionBuiltIn(attribute, function);
                                   return (attribute.Name, (Function)decorator);
                               });
        return this;
    }

    public ReaxInterpreter Build(ReaxNode[] nodes) 
    {
        if(_instanceMain != null)
            return _instanceMain;

        _instanceMain = ReaxInterpreter.CreateMain(nodes, FunctionsBuiltIn);
        return _instanceMain;
    }
}
