using System;
using Reax.Core.Ast;
using Reax.Core.Ast.Expressions;
using Reax.Core.Ast.Interfaces;
using Reax.Core.Ast.Literals;
using Reax.Core.Ast.Statements;
using Reax.Core.Debugger;

namespace Reax.Core;

public interface IReaxInterpreter
{
    IReaxValue? Output { get; }
    IReaxValue? Error { get; }

    void Initialize();
    void Interpret(string identifier, bool rethrow, params IReaxValue[] values);
    void Interpret(bool rethrow = false);
    string PrintStackTrace();

    public void ExecuteFunctionCall(FunctionCallNode functionCall);
}
