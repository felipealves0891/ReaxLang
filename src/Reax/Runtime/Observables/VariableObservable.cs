using Reax.Interpreter;
using Reax.Parser.Node.Expressions;
using Reax.Parser.Node.Interfaces;

namespace Reax.Runtime.Observables;

public class VariableObservable
{
    private readonly ReaxInterpreter _interpreter;

    private readonly BinaryNode? _condition;

    public VariableObservable(ReaxInterpreter interpreter, BinaryNode? condition = null)
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
