using System;
using Reax.Core.Ast;
using Reax.Core.Ast.Literals;
using Reax.Core.Debugger;

namespace Reax.Core;

public interface IReaxInterpreter
{
    Action<DebuggerArgs>? Debug { get; set; }
    LiteralNode? Output { get; }
    LiteralNode? Error { get; }

    void Initialize();
    void Interpret(string identifier, params ReaxNode[] values);
    void Interpret();
    string PrintStackTrace();
}
