using Reax.Interpreter;
using Reax.Core.Ast.Expressions;
using Reax.Core.Ast.Interfaces;
using Reax.Extensions;
using Reax.Core;

namespace Reax.Runtime.Observables;

public class VariableObservable
{
    private readonly IReaxInterpreter _interpreter;

    private readonly BinaryNode? _condition;

    public VariableObservable(IReaxInterpreter interpreter, BinaryNode? condition = null)
    {
        _interpreter = interpreter;
        _condition = condition;
    }

    public virtual bool CanRun(ReaxExecutionContext context) 
    {
        if(_condition is null)
            return true;

        var left = _condition.Left.GetValue(context);
        var right = _condition.Right.GetValue(context);
        var logical = (ILogicOperator)_condition.Operator;
        return logical.Compare(left, right);
    }

    public virtual void Run() 
    {
        _interpreter.Interpret();
    }
}
