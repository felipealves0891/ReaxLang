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
    void Interpret(string identifier, params ReaxNode[] values);
    void Interpret();
    string PrintStackTrace();

    public void ExecuteDeclareBind(BindNode bind);

    public void ExecuteDeclarationScript(ScriptNode script);

    public void ExecuteDeclarationModule(ModuleNode module);

    public void ExecuteFunctionCall(FunctionCallNode functionCall);

    public void ExecuteDeclaration(DeclarationNode declaration);

    public void ExecuteAssignment(AssignmentNode assignment);

    public void ExecuteIf(IfNode node);

    public void ExecuteDeclarationOn(ObservableNode node);

    public void ExecuteDeclarationFunction(FunctionDeclarationNode node);

    public void ExecuteFor(ForNode node);

    public void ExecuteWhile(WhileNode node);

    public void ExecuteExternalFunctionCallNode(ExternalFunctionCallNode node);

    public bool ExecuteBinary(BinaryNode condition);

    public LiteralNode ExecuteContextAndReturnValue(ContextNode node);

    public LiteralNode ExecuteReturn(ReturnSuccessNode returnNode);

    public LiteralNode ExecuteReturn(ReturnErrorNode returnNode);

    public LiteralNode ExecuteMatch(MatchNode match);
    
    public LiteralNode Calculate(CalculateNode node);

}
