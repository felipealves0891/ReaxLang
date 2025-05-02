using System;
using Reax.Parser;
using Reax.Parser.Node;
using Reax.Parser.Node.Expressions;
using Reax.Parser.Node.Literals;
using Reax.Parser.Node.Statements;

namespace Reax.Semantic.Rules;

public class TypeCheckingRule : BaseRule
{
    public TypeCheckingRule()
    {
        Handlers[typeof(AssignmentNode)] = ApplyAssignmentNode;
    }

    private ValidationResult ApplyAssignmentNode(IReaxNode node)
    {
        var assignment = (AssignmentNode)node;
        var expected = GetDataType(assignment.Identifier);
        var current = GetDataType(assignment.Assigned);
        if(current.HasFlag(expected))
            return ValidationResult.Success();
        else
            return ValidationResult.IncompatibleTypes(expected, current, assignment.Location);
    }

    private DataType GetDataType(ReaxNode node)
    {
        if(node is BinaryNode binary)
            return DataType.BOOLEAN;
        else if(node is CalculateNode calculate)
            return DataType.NUMBER;
        else if(node is ExternalFunctionCallNode externalFunction)
            return GetDataTypeByIdentifier(externalFunction.functionCall.Identifier);
        else if(node is FunctionCallNode functionCall)
            return GetDataTypeByIdentifier(functionCall.Identifier);
        else if(node is VarNode var)
            return var.Type;
        else if(node is LiteralNode literal)
            return literal.Type;
        else if(node is MatchNode match)
            return GetResultDataType(match);
        else if(node is ContextNode contextNode)
            return GetDataByContextNode(contextNode);
        else
            return DataType.NONE;
    }

    private DataType GetDataTypeByIdentifier(string identifier)
    {
        var symbol = Context.Resolve(identifier);
        if(symbol is not null)
            return symbol.Type;
        else
            return DataType.NONE;
    }

    private DataType GetResultDataType(MatchNode node) 
    {
        if(node.Success.Type == node.Error.Type)
            return node.Success.Type;

        return DataType.NONE;
    }

    private DataType GetDataByContextNode(ContextNode node)
    {
        foreach (var item in node.Block)
        {
            var type = (DataType)GetDataType(item);
            if(type != DataType.NONE)
                return type;
        }

        return DataType.NONE;
    }

}
