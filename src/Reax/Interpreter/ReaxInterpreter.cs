using System;
using Reax.Parser.Node;
using Reax.Parser.Node.Interfaces;
using Reax.Runtime;
using Reax.Runtime.Functions;

namespace Reax.Interpreter;

public class ReaxInterpreter
{
    public static Stack<ReaxNode> StackTrace = new Stack<ReaxNode>();

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

    public ReaxNode? Output { get; private set; }
    public string Name { get; private set; } = "Main";

    public void DeclareAndSetFunction(string identifier, Function function) 
    {
        _context.Declare(identifier);
        _context.SetFunction(identifier, function);
    }

    public void Initialize() 
    {
        if(_isInitialized)
            return;

        foreach (var node in _nodes)
        {
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
            else if (node is ObservableNode observable)
                ExecuteDeclarationOn(observable);
            else if(node is FunctionNode function)
                ExecuteDeclarationFunction(function);
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
            var value = values[i];
            _context.DeclareImmutable(variable, value);
        }

        Interpret();
    }

    public void Interpret() 
    {
        Initialize();
        
        foreach (var node in _nodes)
        {
            StackTrace.Push(node);

            if(node is FunctionCallNode functionCall)
                Output = ExecuteFunctionCall(functionCall);
            else if (node is AssignmentNode assignment)
                ExecuteAssignment(assignment);
            else if (node is IfNode @if)
                ExecuteIf(@if);
            else if(node is CalculateNode calculate)
                Output = Calculate(calculate);
            else if(node is ReturnNode returnNode)
                Output = ExecuteReturn(returnNode);
            else if(node is ContextNode contextNode)
                Output = ExecuteContextAndReturnValue(contextNode);
            else if(node is ForNode @for)
                ExecuteFor(@for);
            else if(node is WhileNode @while)
                ExecuteWhile(@while);
            else if(node is ExternalFunctionCallNode scriptFunctionCallNode)
                Output = ExecuteExternalFunctionCallNode(scriptFunctionCallNode);

            StackTrace.Pop();
        }
    }

    private void ExecuteDeclareBind(BindNode bind) 
    {
        var interpreter = new ReaxInterpreter($"bind->{bind.Identifier}", [bind.Node], _context);
        _context.Declare(bind.Identifier);
        _context.SetBind(bind.Identifier, interpreter);
    }

    private void ExecuteDeclarationScript(ScriptNode script) 
    {
        script.Interpreter.Interpret();
        _context.Declare(script.Identifier);
        _context.SetScript(script.Identifier, script.Interpreter);
    }

    private void ExecuteDeclarationModule(ModuleNode module)
    {
        _context.Declare(module.identifier);
        _context.SetModule(module.identifier, module.functions);   
    }

    private ReaxNode? ExecuteFunctionCall(FunctionCallNode functionCall) 
    {
        var function = _context.GetFunction(functionCall.Identifier);
        var parameters = functionCall.Parameter.Select(x => GetValue(x)).ToArray();
        return function.Invoke(parameters);
    }

    private void ExecuteDeclaration(DeclarationNode declaration) 
    {
        if(!declaration.Immutable)
        {
            _context.DeclareVariable(declaration.Identifier, declaration.Async);
            if(declaration.Assignment is not null)
                ExecuteAssignment(new AssignmentNode(declaration.Identifier, declaration.Assignment, declaration.Location));    
        }
        else 
        {
            if(declaration.Assignment is null)
                throw new InvalidOperationException("A constante deve ser definida na declaração!");

            _context.DeclareImmutable(declaration.Identifier, new AssignmentNode(declaration.Identifier, declaration.Assignment, declaration.Location));
        }
        
    }

    public void ExecuteAssignment(AssignmentNode assignment)
    {
        if(assignment.Assigned is ContextNode node)
            _context.SetVariable(assignment.Identifier, ExecuteContextAndReturnValue(node));
        else if (assignment.Assigned is VarNode variable)
            _context.SetVariable(assignment.Identifier, _context.GetVariable(variable.Identifier));
        else
            _context.SetVariable(assignment.Identifier, assignment.Assigned);
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
            var interpreter = new ReaxInterpreter(node.ToString(), contextNode.Block, _context);
            interpreter.Interpret();
        }
        else if(node.False is not null)
        {
            var contextNode = (ContextNode)node.False;
            var interpreter = new ReaxInterpreter(node.ToString(), contextNode.Block, _context);
            interpreter.Interpret();
        }
    }

    private void ExecuteDeclarationOn(ObservableNode node) 
    {
        var identifier = node.Var.ToString();
        var contextNode = (ContextNode)node.Block;
        var interpreter = new ReaxInterpreter(node.ToString(), contextNode.Block, _context);
        _context.SetObservable(identifier, interpreter, node.Condition);
    }

    private ReaxNode ExecuteContextAndReturnValue(ContextNode node) 
    {
        var interpreter = new ReaxInterpreter(node.ToString(), node.Block, _context);
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
        var interpreter = new ReaxInterpreter(returnNode.ToString(), block.Block, _context);
        interpreter.Interpret();

        return interpreter.Output ?? throw new InvalidOperationException("Era esperado um retorno!");
    }

    private void ExecuteDeclarationFunction(FunctionNode node) 
    {
        var block = (ContextNode)node.Block;
        var identifier = node.Identifier.ToString();
        var interpreter = new ReaxInterpreter(node.ToString(), block.Block, _context, node.Parameters);
        _context.Declare(identifier);
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
            var interpreter = new ReaxInterpreter(node.ToString(), block.Block, _context);
            interpreter.Interpret();

            var value = _context.GetVariable(declaration.Identifier) as NumberNode;
            if(value is null)
                throw new InvalidOperationException("Não foi possivel obter o controlador do loop");

            var newValue = new NumberNode((value.ValueConverted + 1).ToString(), declaration.Location);
            _context.SetVariable(declaration.Identifier, newValue);
        }
    }

    private void ExecuteWhile(WhileNode node)
    {
        var condition = (BinaryNode)node.condition;
        var block = (ContextNode)node.Block;

        while(ExecuteBinary(condition))
        {
            var interpreter = new ReaxInterpreter(node.ToString(), block.Block, _context);
            interpreter.Interpret();
        }
    }
    
    private ReaxNode? ExecuteExternalFunctionCallNode(ExternalFunctionCallNode node)
    {
        if(_context.ScriptExists(node.scriptName))
        {
            var interpreter = _context.GetScript(node.scriptName);
            var parameters = node.functionCall.Parameter.Select(GetValue).ToArray();
            var identifier = node.functionCall.Identifier;
            return interpreter.ExecuteFunctionCall(new FunctionCallNode(identifier, parameters, node.Location));
        }
        else if(_context.ModuleExists(node.scriptName)) 
        {
            var function = _context.GetModule(node.scriptName, node.functionCall.Identifier);
            var parameters = node.functionCall.Parameter.Select(GetValue).ToArray();
            var identifier = node.functionCall.Identifier;
            return function.Invoke(parameters);
        }

        throw new InvalidOperationException($"Função externa não localizada: {node.scriptName}.{node.functionCall.Identifier}"); 
    }

    private bool ExecuteBinary(BinaryNode condition) 
    {
        var left = condition.Left is BinaryNode 
                 ? new BooleanNode(ExecuteBinary((BinaryNode)condition.Left).ToString(), condition.Location) 
                 : GetValue(condition.Left);

        var right = condition.Right is BinaryNode 
                 ? new BooleanNode(ExecuteBinary((BinaryNode)condition.Right).ToString(), condition.Location) 
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
        else if(node is FunctionCallNode functionCall)
        {
            var function = _context.GetFunction(functionCall.Identifier);
            var parameters = functionCall.Parameter.Select(x => GetValue(x)).ToArray();
            return function.Invoke(parameters) 
                ?? throw new InvalidOperationException($"{functionCall.Location} - função {functionCall.Identifier} não retornou um valor");
        }
        else
            throw new InvalidOperationException("Não foi possivel identificar o tipo da variavel!");
            
    }

    public void PrintStackTrace() {
        foreach (var node in StackTrace.Reverse()) {
            Console.WriteLine($"  at {node.Location.File}:{node.Location.Line}:{node.Location.Position} -> {node.ToString()}");
        }
    }
}
