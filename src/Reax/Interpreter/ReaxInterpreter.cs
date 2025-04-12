using System;
using Reax.Parser.Node;
using Reax.Runtime;

namespace Reax.Interpreter;

public class ReaxInterpreter
{
    private readonly ReaxNode[] _nodes;
    private readonly Dictionary<string, Action<ReaxNode>> _functionBuiltIn;
    private readonly ReaxExecutionContext _context;

    public ReaxInterpreter(ReaxNode[] nodes)
    {
        _nodes = nodes;
        _context = new ReaxExecutionContext();
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
            else if (node is DeclarationNode declaration)
            {
                _context.Declare(declaration.Identifier);
                if(declaration.Assignment is not null)
                    _context.SetValue(declaration.Identifier, declaration.Assignment);
            }
            else if (node is AssignmentNode assignment)
            {
                _context.SetValue(assignment.Identifier, assignment.Assignment);
            }
        }
    }
}
