using System;
using Reax.Interpreter;
using Reax.Parser.Node;
using Reax.Core.Ast.Literals;
using Reax.Core.Functions;
using Reax.Core.Ast;

namespace Reax.Runtime.Functions;

public class InterpreterFunction : Function
{
    private readonly string _identifier;
    private readonly ReaxInterpreter _interpreter;

    public InterpreterFunction(string identifier, ReaxInterpreter interpreter)
    {
        _interpreter = interpreter;
        _identifier = identifier;
    }

    public override (LiteralNode? Success, LiteralNode? Error) Invoke(params ReaxNode[] parameters)
    {
        _interpreter.Interpret(_identifier, parameters);
        return (_interpreter.Output, _interpreter.Error);
    }
}
