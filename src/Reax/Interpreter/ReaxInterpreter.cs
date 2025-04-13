using System;
using Reax.Parser.Node;
using Reax.Runtime;

namespace Reax.Interpreter;

public class ReaxInterpreter
{
    private readonly ReaxNode[] _nodes;
    private readonly Dictionary<string, Action<ReaxNode>> _functionBuiltIn;
    private readonly Dictionary<string, IList<ReaxInterpreter>> _observables;
    private ReaxExecutionContext _context;

    public ReaxInterpreter(ReaxNode[] nodes)
    {
        _nodes = nodes;
        _context = new ReaxExecutionContext();
        _observables = new Dictionary<string, IList<ReaxInterpreter>>();
        _functionBuiltIn = new Dictionary<string, Action<ReaxNode>> 
        {
            {"writer", x => Console.WriteLine(x)}
        };
    }
    
    public ReaxInterpreter(ReaxNode[] nodes, ReaxExecutionContext context)
    {
        _nodes = nodes;
        _context = new ReaxExecutionContext(context);
        _observables = new Dictionary<string, IList<ReaxInterpreter>>();
        _functionBuiltIn = new Dictionary<string, Action<ReaxNode>> 
        {
            {"writer", x => Console.WriteLine(x)}
        };
    }

    public void Interpret() 
    {
        foreach (var node in _nodes)
        {
            if(node is FunctionCallNode functionCall)
                ExecuteFunctionCall(functionCall);
            else if (node is DeclarationNode declaration)
                ExecuteDeclaration(declaration);
            else if (node is AssignmentNode assignment)
                ExecuteAssignment(assignment);
            else if (node is IfNode @if)
                ExecuteIf(@if);
            else if (node is ObservableNode observable)
                ExecuteOn(observable);
        }
    }

    private void ExecuteFunctionCall(FunctionCallNode functionCall) 
    {
        if(_functionBuiltIn.ContainsKey(functionCall.Identifier))
        {
            if(functionCall.Parameter is StringNode)
            {
                _functionBuiltIn[functionCall.Identifier](functionCall.Parameter);
            }
            else if(functionCall.Parameter is VarNode)
            {
                var identifier = functionCall.Parameter.ToString();
                var value = _context.GetValue(identifier);
                _functionBuiltIn[functionCall.Identifier](value);
            }
        }
    }

    private void ExecuteDeclaration(DeclarationNode declaration) 
    {
        _context.Declare(declaration.Identifier);
        if(declaration.Assignment is not null)
            ExecuteAssignment(new AssignmentNode(declaration.Identifier, declaration.Assignment));
    }

    public void ExecuteAssignment(AssignmentNode assignment)
    {
        _context.SetValue(assignment.Identifier, assignment.Assignment);

        if(_observables.TryGetValue(assignment.Identifier, out var interpreters))
        {
            Parallel.ForEach(interpreters, interpreter => interpreter.Interpret());
        }
    }

    private void ExecuteIf(IfNode node) 
    {
        var left = GetValue(node.Condition.Left);
        var right = GetValue(node.Condition.Right);
        var logical = (ILogicOperator)node.Condition.Operator;

        if(logical.Compare(left, right))
        {
            var contextNode = (ContextNode)node.True;
            var interpreter = new ReaxInterpreter(contextNode.Block, _context);
            interpreter.Interpret();
        }
        else if(node.False is not null)
        {
            var contextNode = (ContextNode)node.False;
            var interpreter = new ReaxInterpreter(contextNode.Block, _context);
            interpreter.Interpret();
        }
    }

    private void ExecuteOn(ObservableNode node) 
    {
        var identifier = node.Var.ToString();
        var contextNode = (ContextNode)node.Block;
        var interpreter = new ReaxInterpreter(contextNode.Block, _context);
        if (_observables.TryGetValue(identifier, out var interpreters))
        {
            interpreters.Add(interpreter);
        }
        else
        {
            interpreters = new List<ReaxInterpreter>([interpreter]);
            _observables[identifier] = interpreters;
        }
    }

    private ReaxNode GetValue(ReaxNode node) 
    {
        if(node is NumberNode number)
            return number;
        else if(node is StringNode text)
            return text;
        else if(node is VarNode variable)
            return _context.GetValue(variable.Identifier);
        else
            throw new InvalidOperationException("Não foi possivel identificar o tipo da variavel!");
            
    }
}
