using System;
using Reax.Interpreter;
using Reax.Parser.Node;
using Reax.Core.Ast.Literals;
using Reax.Core.Functions;
using Reax.Core.Ast;
using Reax.Core;
using Reax.Core.Ast.Interfaces;

namespace Reax.Runtime.Functions;

public class InterpreterFunction : Function
{
    private readonly string _identifier;
    private readonly IReaxInterpreter _interpreter;

    public InterpreterFunction(string identifier, IReaxInterpreter interpreter)
    {
        _interpreter = interpreter;
        _identifier = identifier;
    }

    public override (IReaxValue? Success, IReaxValue? Error) Invoke(params IReaxValue[] parameters)
    {
        _interpreter.Interpret(_identifier, true, parameters);
        return (_interpreter.Output, _interpreter.Error);
    }
}
