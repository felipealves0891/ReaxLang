using System;
using System.Reflection;
using Reax.Parser.Node;
using Reax.Runtime.Functions;
using Reax.Runtime.Functions.Attributes;

namespace Reax.Interpreter;

public class ReaxInterpreterBuilder
{
    public string Name { get; init; }
    
    public ReaxInterpreterBuilder(string? name = null)
    {
        Name = name ?? "main";
    }

    public ReaxInterpreter BuildMain(ReaxNode[] nodes) 
    {
        if(ReaxEnvironment.MainInterpreter != null)
            return ReaxEnvironment.MainInterpreter;

        ReaxEnvironment.MainInterpreter = new ReaxInterpreter(nodes);
        return ReaxEnvironment.MainInterpreter;
    }

    public ReaxInterpreter BuildScript(ReaxNode[] nodes) 
        =>  new ReaxInterpreter(Name, nodes);
    
}
