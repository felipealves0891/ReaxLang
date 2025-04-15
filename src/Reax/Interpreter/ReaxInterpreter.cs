using System;
using Reax.Parser.Node;
using Reax.Runtime;
using Reax.Runtime.Functions;

namespace Reax.Interpreter;

public class ReaxInterpreter
{
    private readonly ReaxNode[] _nodes;
    private readonly ReaxExecutionContext _context;
    private readonly Dictionary<int, ReaxNode> _parameters;
    
    private ReaxInterpreter(ReaxNode[] nodes, ReaxExecutionContext context, ReaxNode[] parameters)
        : this(nodes, context)
    {
        for (int i = 0; i < parameters.Length; i++)
            _parameters[i] = parameters[i];
    }

    private ReaxInterpreter(ReaxNode[] nodes, ReaxExecutionContext context)
        : this(nodes)
    {
        _context = new ReaxExecutionContext(context);
    }

    private ReaxInterpreter(ReaxNode[] nodes)
    {
        _nodes = nodes;
        _context = new ReaxExecutionContext();
        _parameters = new Dictionary<int, ReaxNode>();
    }
    
    public static ReaxInterpreter CreateMain(ReaxNode[] nodes, IEnumerable<(string Identifier, Function Function)> functions)
    {
        var interpreter = new ReaxInterpreter(nodes);

        foreach (var fun in functions)
        {
            interpreter._context.DeclareFunction(fun.Identifier);
            interpreter._context.SetFunction(fun.Identifier, fun.Function);
        }

        return interpreter;
    }

    public bool IsOutput => Output is not null;
    public ReaxNode? Output { get; private set; }

    public void Interpret(string identifier, params ReaxNode[] values) 
    {
        var parametersLength = _parameters.Keys.Count();
        if(values.Length != parametersLength)
            throw new InvalidOperationException($"A função {identifier} espera {parametersLength} parametro(s), mas recebeu {values.Length}!");

        for (int i = 0; i < values.Length; i++)
        {
            var variable = _parameters[i].ToString();
            var value = values[i];
            _context.DeclareVariable(variable);
            _context.SetVariable(variable, value);
        }

        Interpret();
    }

    public void Interpret() 
    {
        foreach (var node in _nodes)
        {
            if(node is FunctionCallNode functionCall)
                Output = ExecuteFunctionCall(functionCall);
            else if (node is DeclarationNode declaration)
                ExecuteDeclaration(declaration);
            else if (node is AssignmentNode assignment)
                ExecuteAssignment(assignment);
            else if (node is IfNode @if)
                ExecuteIf(@if);
            else if (node is ObservableNode observable)
                ExecuteOn(observable);
            else if(node is CalculateNode calculate)
                Output = Calculate(calculate);
            else if(node is FunctionNode function)
                ExecuteDeclarationFunction(function);
            else if(node is ReturnNode returnNode)
                Output = ExecuteReturn(returnNode);
            else if(node is ContextNode contextNode)
                Output = ExecuteContextAndReturnValue(contextNode);
            else if(node is ForNode @for)
                ExecuteFor(@for);
            else if(node is WhileNode @while)
                ExecuteWhile(@while);
        }
    }

    private ReaxNode? ExecuteFunctionCall(FunctionCallNode functionCall) 
    {
        var function = _context.GetFunction(functionCall.Identifier);
        var parameters = functionCall.Parameter.Select(x => GetValue(x)).ToArray();
        return function.Invoke(parameters);
    }

    private void ExecuteDeclaration(DeclarationNode declaration) 
    {
        _context.DeclareVariable(declaration.Identifier);
        if(declaration.Assignment is not null)
            ExecuteAssignment(new AssignmentNode(declaration.Identifier, declaration.Assignment));
    }

    public void ExecuteAssignment(AssignmentNode assignment)
    {
        if(assignment.Assignment is ContextNode node)
            _context.SetVariable(assignment.Identifier, ExecuteContextAndReturnValue(node));
        else if (assignment.Assignment is VarNode variable)
            _context.SetVariable(assignment.Identifier, _context.GetVariable(variable.Identifier));
        else
            _context.SetVariable(assignment.Identifier, assignment.Assignment);
    }

    public ReaxNode Calculate(CalculateNode node)
    {
        var op = (IArithmeticOperator)node.Operator;
        var left = GetValue(CalculateChild(node.Left)) as NumberNode;
        var right = GetValue(CalculateChild(node.Right)) as NumberNode;

        if(left is null)
            throw new InvalidOperationException($"Valor invalido para calculo '{left}'");
        
        if(right is null)
            throw new InvalidOperationException($"Valor invalido para calculo '{right}'");

        return op.Calculate(left, right);
    }

    private ReaxNode CalculateChild(ReaxNode node) 
    {
        if(node is CalculateNode calculate)
            return Calculate(calculate);
        else if (node is IReaxValue value)
            return node;
        else 
            throw new InvalidOperationException("Não é possivel tratar o nó da operação!");
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
        _context.SetObservable(identifier, interpreter, node.Condition);
    }

    private ReaxNode ExecuteContextAndReturnValue(ContextNode node) 
    {
        var interpreter = new ReaxInterpreter(node.Block, _context);
        interpreter.Interpret();

        if(interpreter.Output is null)
            throw new InvalidOperationException("Era esperado que a função retornace um valor!");

        return interpreter.Output;
    }

    private ReaxNode ExecuteReturn(ReturnNode returnNode) 
    {
        if(returnNode.Expression is IReaxValue)
            return GetValue(returnNode.Expression);
        
        var block = (ContextNode)returnNode.Expression;
        var interpreter = new ReaxInterpreter(block.Block, _context);
        interpreter.Interpret();

        return interpreter.Output ?? throw new InvalidOperationException("Era esperado um retorno!");
    }

    private void ExecuteDeclarationFunction(FunctionNode node) 
    {
        var block = (ContextNode)node.Block;
        var identifier = node.Identifier.ToString();
        var interpreter = new ReaxInterpreter(block.Block, _context, node.Parameters);
        _context.DeclareFunction(identifier);
        _context.SetFunction(identifier, interpreter);
    }

    private void ExecuteFor(ForNode node) 
    {
        var declaration = (DeclarationNode)node.declaration;
        var condition = (BinaryNode)node.condition;
        var block = (ContextNode)node.Block;

        ExecuteDeclaration(declaration);
        while(ExecuteBinary(condition))
        {
            var interpreter = new ReaxInterpreter(block.Block, _context);
            interpreter.Interpret();

            var value = _context.GetVariable(declaration.Identifier) as NumberNode;
            if(value is null)
                throw new InvalidOperationException("Não foi possivel obter o controlador do loop");

            var newValue = new NumberNode((value.ValueConverted + 1).ToString());
            _context.SetVariable(declaration.Identifier, newValue);
        }
    }

    private void ExecuteWhile(WhileNode node)
    {
        var condition = (BinaryNode)node.condition;
        var block = (ContextNode)node.Block;

        while(ExecuteBinary(condition))
        {
            var interpreter = new ReaxInterpreter(block.Block, _context);
            interpreter.Interpret();
        }
    }

    private bool ExecuteBinary(BinaryNode condition) 
    {
        var left = condition.Left is BinaryNode 
                 ? new BooleanNode(ExecuteBinary((BinaryNode)condition.Left).ToString()) 
                 : GetValue(condition.Left);

        var right = condition.Right is BinaryNode 
                 ? new BooleanNode(ExecuteBinary((BinaryNode)condition.Right).ToString()) 
                 : GetValue(condition.Right);

        var logical = (ILogicOperator)condition.Operator;
        return logical.Compare(left, right);
    }

    private ReaxNode GetValue(ReaxNode node) 
    {
        if(node is NumberNode number)
            return number;
        else if(node is StringNode text)
            return text;
        else if(node is VarNode variable)
            return _context.GetVariable(variable.Identifier);
        else
            throw new InvalidOperationException("Não foi possivel identificar o tipo da variavel!");
            
    }
}
