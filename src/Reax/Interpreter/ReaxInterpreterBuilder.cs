using Reax.Core;
using Reax.Core.Ast;
using Reax.Core.Debugger;

namespace Reax.Interpreter;

public class ReaxInterpreterBuilder
{
    public string Name { get; init; }
    
    public ReaxInterpreterBuilder(string? name = null)
    {
        Name = name ?? "main";
    }

    public IReaxInterpreter BuildMain(ReaxNode[] nodes) 
    {
        if(ReaxEnvironment.MainInterpreter != null)
            return ReaxEnvironment.MainInterpreter;

        ReaxEnvironment.MainInterpreter = new ReaxInterpreter(nodes);
        return ReaxEnvironment.MainInterpreter;
    }

    public ReaxInterpreter BuildScript(ReaxNode[] nodes) 
    {
        var interpreter = new ReaxInterpreter(Name, nodes);
        return interpreter;
    }
}
