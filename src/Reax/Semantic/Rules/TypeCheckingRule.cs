using System;
using Reax.Parser;
using Reax.Parser.Node;
using Reax.Parser.Node.Expressions;
using Reax.Parser.Node.Literals;
using Reax.Parser.Node.Statements;
using Reax.Semantic.Contexts;

namespace Reax.Semantic.Rules;

public class TypeCheckingRule : BaseRule
{
    public TypeCheckingRule()
    {
        Handlers[typeof(AssignmentNode)] = ApplyAssignmentNode;
        Handlers[typeof(ActionNode)] = ApplyActionNode;
        Handlers[typeof(FunctionDeclarationNode)] = ApplyFunctionDeclarationNode;
        Handlers[typeof(FunctionCallNode)] = ApplyFunctionCallNode;
        Handlers[typeof(ExternalFunctionCallNode)] = ApplyExternalFunctionCallNode;
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

    private ValidationResult ApplyActionNode(IReaxNode node)
    {
        var action = (ActionNode)node;
        var expected = action.Type;
        var current = GetDataType(action.Context);
        if(current.HasFlag(expected))
            return ValidationResult.Success();
        else
            return ValidationResult.IncompatibleTypes(expected, current, action.Location);
    }

    private ValidationResult ApplyFunctionDeclarationNode(IReaxNode node)
    {
        var declarationNode = (FunctionDeclarationNode)node;
        var expected = declarationNode.SuccessType | declarationNode.ErrorType;
        var current = GetDataType(declarationNode.Block);
        if((expected & current) == current)
            return ValidationResult.Success();
        else
            return ValidationResult.IncompatibleTypes(declarationNode.SuccessType | declarationNode.ErrorType, current, declarationNode.Location);
    }

    private ValidationResult ApplyFunctionCallNode(IReaxNode node)
    {
        var call = (FunctionCallNode)node;
        var symbol = Context.Resolve(call.Identifier);
        if (symbol is null)
            return ValidationResult.SymbolUndeclared(call.Identifier, call.Location);
        
        var expectedParameters = Context.ResolveParameters(call.Identifier);
        var passedParameters = call.Parameter;

        return ValidateParameters(
            passedParameters,
            expectedParameters,
            call.Location,
            call.Identifier,
            "main");
    }

    private ValidationResult ApplyExternalFunctionCallNode(IReaxNode node)
    {
        var external = (ExternalFunctionCallNode)node;
        var call = external.functionCall;
        var symbol = Context.Resolve(call.Identifier, external.scriptName);
        if (symbol is null)
            return ValidationResult.SymbolUndeclared(call.Identifier, call.Location);
        
        var expectedParameters = Context.ResolveParameters(call.Identifier, external.scriptName);
        var passedParameters = call.Parameter;

        return ValidateParameters(
            passedParameters,
            expectedParameters,
            external.Location,
            call.Identifier,
            external.scriptName);
    }
    
    private ValidationResult ValidateParameters(
        ReaxNode[] passedParameters, 
        Symbol[] expectedParameters,
        SourceLocation location,
        string identifier,
        string script) 
    {
        var requiredParameters = expectedParameters.Count(x => x.Category == SymbolCategory.PARAMETER);
        if(passedParameters.Length > expectedParameters.Length || passedParameters.Length < requiredParameters)
            return ValidationResult.InvalidFunctionCall_ParametersCount(
                $"{script}.{identifier}", 
                expectedParameters.Length, 
                passedParameters.Length, 
                location);

        for (int i = 0; i < passedParameters.Length; i++)
        {
            DataType passedType = GetDataType(passedParameters[i]);
            if(!expectedParameters[i].Type.IsCompatatible(passedType))
                return ValidationResult.InvalidFunctionCall_InvalidParameter(
                    $"{script}.{identifier}", 
                    expectedParameters[i].Identifier,
                    expectedParameters[i].Type, 
                    passedType, 
                    location);
        }

        return ValidationResult.Success();
    }

    private DataType GetDataType(ReaxNode node)
    {
        if(node is BinaryNode)
            return DataType.BOOLEAN;
        else if(node is CalculateNode)
            return DataType.NUMBER;
        else if(node is ExternalFunctionCallNode externalFunction)
            return GetDataTypeByIdentifier(externalFunction.functionCall.Identifier, externalFunction.scriptName);
        else if(node is FunctionCallNode functionCall)
            return GetDataTypeByIdentifier(functionCall.Identifier);
        else if(node is VarNode var)
            return var.Type != DataType.NONE ? var.Type : GetDataTypeByIdentifier(var.Identifier);
        else if(node is LiteralNode literal)
            return literal.Type;
        else if(node is MatchNode match)
            return GetResultDataType(match);
        else if(node is ContextNode contextNode)
            return GetDataByContextNode(contextNode);
        else if(node is ReturnSuccessNode successNode)
            return GetDataTypeByReturn(successNode);
        else if(node is ReturnErrorNode errorNode)
            return GetDataTypeByReturn(errorNode);
        else if(node is IfNode ifNode)
            return GetDataTypeByIf(ifNode);
        else
            return DataType.NONE;
    }

    private DataType GetDataTypeByIdentifier(string identifier, string? script = null)
    {
        var symbol = Context.Resolve(identifier, script);
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
        DataType returnType = DataType.NONE;
        foreach (var item in node.Block)
        {
            var type = (DataType)GetDataType(item);
            if(type != DataType.NONE)
            {
                if(returnType == DataType.NONE)
                    returnType = type;
                else
                    returnType = returnType | type;
            }
        }

        return returnType;
    }

    private DataType GetDataTypeByReturn(ReturnSuccessNode successNode) 
    {
        if(successNode.Expression is ContextNode context)
            return GetDataByContextNode(context);
        else if(successNode.Expression is VarNode var)
            return GetDataType(var);
        else if(successNode.Expression is LiteralNode literal)
            return literal.Type;
        else
            return DataType.NONE;
    }

    private DataType GetDataTypeByReturn(ReturnErrorNode successNode) 
    {
        if(successNode.Expression is ContextNode context)
            return GetDataByContextNode(context);
        else if(successNode.Expression is VarNode var)
            return GetDataType(var);
        else if(successNode.Expression is LiteralNode literal)
            return literal.Type;
        else
            return DataType.NONE;
    }

    private DataType GetDataTypeByIf(IfNode node)
    {
        var type = GetDataType(node.True);
        if(node.False is null)
            return type;

        type = type | GetDataType(node.False);
        return type;
    }
}
