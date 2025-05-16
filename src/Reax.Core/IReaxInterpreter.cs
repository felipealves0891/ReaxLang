using System;
using Reax.Core.Ast;
using Reax.Core.Ast.Expressions;
using Reax.Core.Ast.Literals;
using Reax.Core.Ast.Statements;
using Reax.Core.Debugger;

namespace Reax.Core;

public interface IReaxInterpreter
{
    Action<DebuggerArgs>? Debug { get; set; }
    LiteralNode? Output { get; }
    LiteralNode? Error { get; }

    void Initialize();
    void Interpret(string identifier, bool rethrow, params ReaxNode[] values);
    void Interpret(bool rethrow = false);
    string PrintStackTrace();

    public void ExecuteFunctionCall(FunctionCallNode functionCall);
}
