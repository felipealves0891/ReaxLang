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
using System.Runtime.CompilerServices;

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
        : this(name, nodes)
    {
        _context = new ReaxExecutionContext(name, context);
    }

    public ReaxInterpreter(string name, ReaxNode[] nodes)
    {
        Name = name;
        _nodes = nodes;
        _context = new ReaxExecutionContext(name);
        _parameters = new Dictionary<int, ReaxNode>();
    }

    public ReaxInterpreter(ReaxNode[] nodes)
    {
        Name = "Main";
        _nodes = nodes;
        _context = new ReaxExecutionContext("main");
        _parameters = new Dictionary<int, ReaxNode>();
    }

    public Action<DebuggerArgs>? Debug { get; set; }
    public IReaxValue? Output { get; private set; }
    public IReaxValue? Error { get; private set; }
    public string Name { get; private set; }

    public void Initialize() 
    {
        Output = null;
        Error = null;
        
        if(_isInitialized)
            return;

        foreach (var node in _nodes)
        {
            if (node is IReaxDeclaration declaration)
            {
                Logger.LogInterpreter($"Adicionando {node} a stack!");
                StackTrace.Push(node);
                declaration.Execute(_context);
                Logger.LogInterpreter($"Removendo {node} a stack!");
                StackTrace.TryPop(out var _);
            }
        }

        _isInitialized = true;
    }

    public void Interpret(string identifier, bool rethrow, params IReaxValue[] values) 
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

        Interpret(rethrow);
    }

    public void Interpret(bool rethrow = false)
    {
        Initialize();
        Logger.LogInterpreter($"################################### Start {Name} ###################################");
        foreach (var node in _nodes)
        {
            ProcessNode(node, rethrow);
            if ((Output is not null and not NullNode) || (Error is not null and not NullNode)) break;
        }
        Logger.LogInterpreter($"#################################### END  {Name} ###################################");
    }

    private void ProcessNode(ReaxNode node, bool rethrow = false)
    {
        try
        {
            TryingProcessNode(node);
        }
        catch (ReturnSuccessException success)
        {
            if (rethrow)
                throw;

            Output = success.Value;
        }
        catch (ReturnErrorException error)
        {
            if (rethrow)
                throw;
                
            Error = error.Value;
        }
    }

    private void TryingProcessNode(ReaxNode node)
    {
        Logger.LogInterpreter($"Adicionando {node} a stack!");
        StackTrace.Push(node);

        if (node is StatementNode statement and not IReaxDeclaration)
            statement.Execute(_context);
        else if (node is ExpressionNode expression and not IReaxDeclaration)
            Output = expression.Evaluation(_context);
        else if (node is LiteralNode literal and not IReaxDeclaration)
            Output = literal;

        if(ReaxEnvironment.Debug)
            OnDebug(node.Location);

        Logger.LogInterpreter($"Removendo {node} a stack!");
        StackTrace.TryPop(out var nodeOut);
    } 

    public void ExecuteFunctionCall(FunctionCallNode functionCall)
    {
        try
        {
            var function = _context.GetFunction(functionCall.Identifier);
            var parameters = functionCall.Parameter.Select(x => x.GetValue(_context)).ToArray();
            var (success, error) = function.Invoke(parameters);
            Output = success;
            Error = error;    
        }
        catch (ReturnSuccessException ex)
        {
            Output = ex.Value;
        }
        catch (ReturnErrorException ex)
        {
            Error = ex.Value;
        }
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
