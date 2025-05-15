using System;
using System.Collections.Concurrent;
using Reax.Core.Locations;
using Reax.Core.Debugger;
using Reax.Parser;
using Reax.Parser.Node;
using Reax.Core.Ast.Expressions;
using Reax.Core.Ast.Interfaces;
using Reax.Core.Ast.Literals;
using Reax.Core.Ast.Statements;
using Reax.Runtime;
using Reax.Runtime.Functions;
using System.Text;
using Reax.Core.Ast;
using Reax.Extensions;
using Reax.Core.Functions;
using Reax.Core;

namespace Reax.Interpreter;

public class ReaxInterpreter : IReaxInterpreter
{
    public static ConcurrentStack<ReaxNode> StackTrace = new ConcurrentStack<ReaxNode>();
    private readonly ReaxNode[] _nodes;
    private readonly ReaxExecutionContext _context;
    private readonly Dictionary<int, ReaxNode> _parameters;
    private bool _isInitialized = false;

    public ReaxInterpreter(string name, ReaxNode[] nodes, ReaxExecutionContext context, ReaxNode[] parameters)
        : this(name, nodes, context)
    {
        for (int i = 0; i < parameters.Length; i++)
            _parameters[i] = parameters[i];
    }

    public ReaxInterpreter(string name, ReaxNode[] nodes, ReaxExecutionContext context)
        : this(nodes)
    {
        _context = new ReaxExecutionContext(name, context);
    }

    public ReaxInterpreter(string name, ReaxNode[] nodes)
    {
        _nodes = nodes;
        _context = new ReaxExecutionContext(name);
        _parameters = new Dictionary<int, ReaxNode>();
    }

    public ReaxInterpreter(ReaxNode[] nodes)
    {
        _nodes = nodes;
        _context = new ReaxExecutionContext("main");
        _parameters = new Dictionary<int, ReaxNode>();
    }

    public Action<DebuggerArgs>? Debug { get; set; }
    public LiteralNode? Output { get; private set; }
    public LiteralNode? Error { get; private set; }
    public string Name { get; private set; } = "Main";

    public void Initialize() 
    {
        Output = null;
        Error = null;
        
        if(_isInitialized)
            return;

        foreach (var node in _nodes)
        {
            Logger.LogInterpreter(node.Location.ToString());
            Logger.LogInterpreter($"Adicionando {node} a stack!");
            StackTrace.Push(node);
            Logger.LogInterpreter($"Adicionado {node} a stack!");

            if(node is ScriptDeclarationNode scriptDeclaration)
                Name = scriptDeclaration.Identifier;
            else if(node is ScriptNode script)
                ExecuteDeclarationScript(script);
            else if(node is ModuleNode module)
                ExecuteDeclarationModule(module);
            else if(node is BindNode bind)
                ExecuteDeclareBind(bind);
            else if (node is DeclarationNode declaration)
                ExecuteDeclaration(declaration);
            else if(node is FunctionDeclarationNode function)
                ExecuteDeclarationFunction(function);

            Logger.LogInterpreter($"Removendo {node} a stack!");
            StackTrace.TryPop(out var _);
            Logger.LogInterpreter($"Removido {node} a stack!");
        }

        _isInitialized = true;
    }

    public void Interpret(string identifier, params ReaxNode[] values) 
    {
        var parametersLength = _parameters.Keys.Count();
        if(values.Length != parametersLength)
            throw new InvalidOperationException($"A função {identifier} espera {parametersLength} parametro(s), mas recebeu {values.Length}!");

        for (int i = 0; i < values.Length; i++)
        {
            var variable = _parameters[i].ToString();
            var value = (LiteralNode)values[i];
            _context.DeclareImmutable(variable, value);
        }

        Interpret();
    }

    public void Interpret() 
    {
        Initialize();
        
        foreach (var node in _nodes)
        {
            Logger.LogInterpreter(node.Location.ToString());
            Logger.LogInterpreter($"Adicionando {node} a stack!");
            StackTrace.Push(node);
            Logger.LogInterpreter($"Adicionando {node} a stack!");

            if(node is FunctionCallNode functionCall)
                ExecuteFunctionCall(functionCall);
            else if (node is AssignmentNode assignment)
                ExecuteAssignment(assignment);
            else if (node is IfNode @if)
                ExecuteIf(@if);
            else if(node is CalculateNode calculate)
                Output = Calculate(calculate);
            else if(node is ReturnSuccessNode returnSuccessNode)
                Output = ExecuteReturn(returnSuccessNode);
            else if(node is ReturnErrorNode returnErrorNode)
                Error = ExecuteReturn(returnErrorNode);
            else if(node is ContextNode contextNode)
                Output = ExecuteContextAndReturnValue(contextNode);
            else if(node is ForNode @for)
                ExecuteFor(@for);
            else if(node is WhileNode @while)
                ExecuteWhile(@while);
            else if(node is ExternalFunctionCallNode scriptFunctionCallNode)
                ExecuteExternalFunctionCallNode(scriptFunctionCallNode);
            else if(node is BinaryNode binary)
                Output = new BooleanNode(ExecuteBinary(binary).ToString(), binary.Location);
            else if(node is MatchNode match)
                Output = ExecuteMatch(match);
            else if (node is ObservableNode observable)
                ExecuteDeclarationOn(observable);
            else if(node is IReaxValue)
                Output = node.GetValue(_context);

            if(ReaxEnvironment.Debug)
                OnDebug(node.Location);

            Logger.LogInterpreter($"Removendo {node} a stack!");
            StackTrace.TryPop(out var nodeOut);
            Logger.LogInterpreter($"Removido {nodeOut} a stack!");

            if(Output is not null || Error is not null)
                break;
        }
    }

    public void ExecuteDeclareBind(BindNode bind) 
    {
        var interpreter = new ReaxInterpreter($"bind->{bind.Identifier}", [bind.Node.Assigned], _context);
        interpreter.Debug += ReaxDebugger.Debugger;
        _context.Declare(bind.Identifier);
        _context.SetBind(bind.Identifier, interpreter);
    }

    public void ExecuteDeclarationScript(ScriptNode script) 
    {
        var interpreter = new ReaxInterpreter(script.Nodes);
        interpreter.Interpret();

        _context.Declare(interpreter.Name);
        _context.SetScript(script.Identifier, interpreter);
    }

    public void ExecuteDeclarationModule(ModuleNode module)
    {
        _context.Declare(module.identifier);
        _context.SetModule(module.identifier, module.functions);   
    }

    public void ExecuteFunctionCall(FunctionCallNode functionCall) 
    {
        var function = _context.GetFunction(functionCall.Identifier);
        var parameters = functionCall.Parameter.Select(x => x.GetValue(_context)).ToArray();
        var (success, error) = function.Invoke(parameters);
        Output = success;
        Error = error;
    }

    public void ExecuteDeclaration(DeclarationNode declaration) 
    {
        if(!declaration.Immutable)
        {
            _context.DeclareVariable(declaration.Identifier, declaration.Async);
            if(declaration.Assignment is not null && declaration.Assignment is not AssignmentNode)
                ExecuteAssignment(new AssignmentNode(new VarNode(declaration.Identifier, declaration.Type, declaration.Location), declaration.Assignment, declaration.Location));    
            else if(declaration.Assignment is not null && declaration.Assignment is AssignmentNode assignment)
                ExecuteAssignment(assignment);    
        }
        else 
        {
            if(declaration.Assignment is null)
                throw new InvalidOperationException("A constante deve ser definida na declaração!");

            if(declaration.Assignment is AssignmentNode assignment)
                _context.DeclareImmutable(declaration.Identifier, assignment.Assigned.GetValue(_context));
            else
                _context.DeclareImmutable(declaration.Identifier, declaration.Assignment.GetValue(_context));
        }
        
    }

    public void ExecuteAssignment(AssignmentNode assignment)
    {
        if(assignment.Assigned is ContextNode node)
            _context.SetVariable(assignment.Identifier.Identifier, ExecuteContextAndReturnValue(node));
        else if (assignment.Assigned is VarNode variable)
            _context.SetVariable(assignment.Identifier.Identifier, _context.GetVariable(variable.Identifier));
        else if (assignment.Assigned is MatchNode match)
            _context.SetVariable(assignment.Identifier.Identifier, ExecuteMatch(match));
        else if(assignment.Assigned is LiteralNode literal)
            _context.SetVariable(assignment.Identifier.Identifier, literal);
        else 
        {
            var contextNode = new ContextNode([assignment.Assigned], assignment.Assigned.Location);
            _context.SetVariable(assignment.Identifier.Identifier, ExecuteContextAndReturnValue(contextNode));
        }
    }

    public LiteralNode Calculate(CalculateNode node)
    {
        var op = (IArithmeticOperator)node.Operator;
        var left = CalculateChild(node.Left).GetValue(_context) as NumberNode;
        var right = CalculateChild(node.Right).GetValue(_context) as NumberNode;

        if(left is null)
            throw new InvalidOperationException($"Valor invalido para calculo '{left}'");
        
        if(right is null)
            throw new InvalidOperationException($"Valor invalido para calculo '{right}'");

        return op.Calculate(left, right);
    }

    public ReaxNode CalculateChild(ReaxNode node) 
    {
        if(node is CalculateNode calculate)
            return Calculate(calculate);
        else if (node is IReaxValue)
            return node;
        else 
            throw new InvalidOperationException("Não é possivel tratar o nó da operação!");
    }

    public void ExecuteIf(IfNode node) 
    {
        var left = node.Condition.Left.GetValue(_context);
        var right = node.Condition.Right.GetValue(_context);
        var logical = (ILogicOperator)node.Condition.Operator;
        var result = logical.Compare(left, right);
        if(result)
        {
            var contextNode = node.True;
            var interpreter = new ReaxInterpreter(node.ToString(), contextNode.Block, _context);
            interpreter.Debug += ReaxDebugger.Debugger;
            interpreter.Interpret();
            Output = interpreter.Output;
            Error = interpreter.Error;
        }
        else if(node.False is not null)
        {
            var contextNode = node.False;
            var interpreter = new ReaxInterpreter(node.ToString(), contextNode.Block, _context);
            interpreter.Debug += ReaxDebugger.Debugger;
            interpreter.Interpret();
            Output = interpreter.Output;
            Error = interpreter.Error;
        }
    }

    public void ExecuteDeclarationOn(ObservableNode node) 
    {
        var identifier = node.Var.ToString();
        var contextNode = (ContextNode)node.Block;
        var interpreter = new ReaxInterpreter(node.ToString(), contextNode.Block, _context);
        interpreter.Debug += ReaxDebugger.Debugger;
        _context.SetObservable(identifier, interpreter, node.Condition);
    }

    public LiteralNode ExecuteContextAndReturnValue(ContextNode node) 
    {
        var interpreter = new ReaxInterpreter(node.ToString(), node.Block, _context);
        interpreter.Debug += ReaxDebugger.Debugger;
        interpreter.Interpret();

        if(interpreter.Output is null)
            throw new InvalidOperationException("Era esperado que a função retornace um valor!");

        return interpreter.Output;
    }

    public LiteralNode ExecuteReturn(ReturnSuccessNode returnNode) 
    {
        if(returnNode.Expression is IReaxValue)
            return returnNode.Expression.GetValue(_context);
        
        var interpreter = new ReaxInterpreter(returnNode.ToString(), [returnNode.Expression], _context);
        interpreter.Debug += ReaxDebugger.Debugger;
        interpreter.Interpret();

        return interpreter.Output ?? throw new InvalidOperationException("Era esperado um retorno!");
    }
    
    public LiteralNode ExecuteReturn(ReturnErrorNode returnNode) 
    {
        if(returnNode.Expression is IReaxValue)
            return returnNode.Expression.GetValue(_context);
        
        var block = (ContextNode)returnNode.Expression;
        var interpreter = new ReaxInterpreter(returnNode.ToString(), block.Block, _context);
        interpreter.Debug += ReaxDebugger.Debugger;
        interpreter.Interpret();

        return interpreter.Output ?? throw new InvalidOperationException("Era esperado um retorno!");
    }

    public void ExecuteDeclarationFunction(FunctionDeclarationNode node) 
    {
        var block = node.Block;
        var identifier = node.Identifier;
        var interpreter = new ReaxInterpreter(node.ToString(), block.Block, _context, node.Parameters);
        interpreter.Debug += ReaxDebugger.Debugger;
        _context.Declare(identifier);
        _context.SetFunction(identifier, interpreter);
    }

    public void ExecuteFor(ForNode node) 
    {
        var declaration = node.Declaration;
        var condition = (BinaryNode)node.Condition;
        var block = node.Block;

        ExecuteDeclaration(declaration);
        while(ExecuteBinary(condition))
        {
            var interpreter = new ReaxInterpreter(node.ToString(), block.Block, _context);
            interpreter.Debug += ReaxDebugger.Debugger;
            interpreter.Interpret();

            var value = _context.GetVariable(declaration.Identifier) as NumberNode;
            if(value is null)
                throw new InvalidOperationException("Não foi possivel obter o controlador do loop");

            var newValue = new NumberNode(((decimal)value.Value + 1).ToString(), declaration.Location);
            _context.SetVariable(declaration.Identifier, newValue);
        }
    }

    public void ExecuteWhile(WhileNode node)
    {
        var condition = (BinaryNode)node.Condition;
        var block = (ContextNode)node.Block;

        while(ExecuteBinary(condition))
        {
            var interpreter = new ReaxInterpreter(node.ToString(), block.Block, _context);
            interpreter.Debug += ReaxDebugger.Debugger;
            interpreter.Interpret();
        }
    }
    
    public void ExecuteExternalFunctionCallNode(ExternalFunctionCallNode node)
    {
        if(_context.ScriptExists(node.scriptName))
        {
            var interpreter = _context.GetScript(node.scriptName);
            var parameters = node.functionCall.Parameter.Select(x => x.GetValue(_context)).ToArray();
            var identifier = node.functionCall.Identifier;
            interpreter.ExecuteFunctionCall(new FunctionCallNode(identifier, parameters, node.Location));
            Output = interpreter.Output;
            Error = interpreter.Error;
        }
        else if(_context.ModuleExists(node.scriptName)) 
        {
            var function = _context.GetModule(node.scriptName, node.functionCall.Identifier);
            var parameters = node.functionCall.Parameter.Select(x => x.GetValue(_context)).ToArray();
            var identifier = node.functionCall.Identifier;
            var (success, error) = function.Invoke(parameters);
            Output = success;
            Error = error;
        }
        else 
        {
            throw new InvalidOperationException($"Função externa não localizada: {node.scriptName}.{node.functionCall.Identifier}"); 
        }
    }

    public bool ExecuteBinary(BinaryNode condition) 
    {
        var left = condition.Left is BinaryNode 
                 ? new BooleanNode(ExecuteBinary((BinaryNode)condition.Left).ToString(), condition.Location) 
                 : condition.Left.GetValue(_context);

        var right = condition.Right is BinaryNode 
                 ? new BooleanNode(ExecuteBinary((BinaryNode)condition.Right).ToString(), condition.Location) 
                 : condition.Right.GetValue(_context);

        var logical = (ILogicOperator)condition.Operator;
        return logical.Compare(left, right);
    }

    public LiteralNode ExecuteMatch(MatchNode match) 
    {
        var expressionInterpreter = new ReaxInterpreter(match.Expression.ToString(), [match.Expression], _context);
        expressionInterpreter.Interpret();

        if(expressionInterpreter.Output is not null)
        {
            var successInterpreter = new ReaxInterpreter(match.Success.ToString(), [match.Success.Context], _context, [match.Success.Parameter]);
            successInterpreter.Debug += ReaxDebugger.Debugger;
            successInterpreter.Interpret("Success", [expressionInterpreter.Output]);
            return successInterpreter.Output ?? throw new InvalidOperationException($"{match.Location} - Era esperado um retorno de sucesso ou erro, mas não teve nenhum retorno!");
        }
        else if (expressionInterpreter.Error is not null)
        {
            var errorInterpreter = new ReaxInterpreter(match.Error.ToString(), [match.Error.Context], _context, [match.Error.Parameter]);
            errorInterpreter.Interpret("Error", [expressionInterpreter.Error]);
            errorInterpreter.Debug += ReaxDebugger.Debugger;
            return errorInterpreter.Output ?? throw new InvalidOperationException($"{match.Location} - Era esperado um retorno de sucesso ou erro, mas não teve nenhum retorno!");
        }
        else 
            throw new InvalidOperationException($"{match.Location} - Era esperado um retorno de sucesso ou erro, mas não teve nenhum retorno!");
    }

    private void OnDebug(SourceLocation location) 
    {
        Debug?.Invoke(new DebuggerArgs(_context.Debug(), PrintStackTrace(), location));
    }

    public string PrintStackTrace() {
        if(!StackTrace.Any()) return "";
        
        StringBuilder sb = new();
        foreach (var node in StackTrace.Reverse()) {
            sb.Append($"  at {node.Location.File}:{node.Location.Start.Line}:{node.Location.Start.Column} -> {node.ToString()}");
        }
        return sb.ToString();
    }
}
