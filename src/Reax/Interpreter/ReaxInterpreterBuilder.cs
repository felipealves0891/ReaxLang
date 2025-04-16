using System;
using System.Reflection;
using Reax.Parser.Node;
using Reax.Runtime.Functions;
using Reax.Runtime.Functions.Attributes;

namespace Reax.Interpreter;

public class ReaxInterpreterBuilder
{
    public IEnumerable<(string identifier, Function function)> FunctionsBuiltIn { get; private set; }
    public string Name { get; init; }
    
    public ReaxInterpreterBuilder(string? name = null)
    {
        FunctionsBuiltIn = Enumerable.Empty<(string identifier, Function function)>();
        Name = name ?? "main";
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

    public ReaxInterpreter BuildMain(ReaxNode[] nodes) 
    {
        if(ReaxEnvironment.MainInterpreter != null)
            return ReaxEnvironment.MainInterpreter;

        ReaxEnvironment.MainInterpreter = ReaxInterpreter.CreateMain(nodes, FunctionsBuiltIn);
        return ReaxEnvironment.MainInterpreter;
    }

    public ReaxInterpreter BuildModule(ReaxNode[] nodes) 
        =>  ReaxInterpreter.Create(Name, nodes, FunctionsBuiltIn);
    
}
