using System;
using Reax.Core.Ast;
using Reax.Core.Ast.Expressions;
using Reax.Core.Ast.Interfaces;
using Reax.Core.Ast.Literals;
using Reax.Core.Debugger;
using Reax.Core.Functions;

namespace Reax.Core;

public interface IReaxExecutionContext
{
    IReaxInterpreter CreateInterpreter(string name, ReaxNode[] nodes);
    IReaxInterpreter CreateInterpreter(string name, ReaxNode[] nodes, VarNode[] Parameters);
    bool ScriptExists(string identifier);
    bool ModuleExists(string identifier);
    void DeclareVariable(string identifier, bool isAsync);
    void DeclareImmutable(string identifier, IReaxValue node);
    void Declare(string identifier);
    void SetVariable(string identifier, IReaxValue value);
    void SetFunction(string identifier, IReaxInterpreter value);
    void SetFunction(string identifier, Function value);
    void SetObservable(string identifier, IReaxInterpreter interpreter, BinaryNode? condition);
    void SetScript(string identifier, IReaxInterpreter interpreter);
    void SetModule(string identifier, Dictionary<string, Function> functions);
    void SetBind(string identifier, IReaxInterpreter interpreter);
    bool Remove(string identifier);
    IReaxValue GetVariable(string identifier);
    IReaxValue? GetBind(string identifier);
    Function GetFunction(string identifier);
    IReaxInterpreter GetScript(string identifier);
    Function GetModule(string identifier, string functionName);
}
