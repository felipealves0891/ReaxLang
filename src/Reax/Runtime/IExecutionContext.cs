using System;
using Reax.Interpreter;
using Reax.Parser.Node;
using Reax.Runtime.Functions;

namespace Reax.Runtime;

public interface IExecutionContext
{
    public IExecutionContext GetParent();

    public void DeclareVariable(string identifier);

    public void DeclareFunction(string identifier);

    public void SetVariable(string identifier, ReaxNode value);

    public void SetFunction(string identifier, ReaxInterpreter value);

    public void SetFunction(string identifier, Function value);

    public void SetObservable(string identifier, ReaxInterpreter interpreter, BinaryNode? condition);

    public ReaxNode GetVariable(string identifier);

    public Function GetFunction(string identifier);
    
}
