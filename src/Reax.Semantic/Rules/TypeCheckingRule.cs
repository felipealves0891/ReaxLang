using System;
using Reax.Core.Locations;
using Reax.Core.Types;
using Reax.Parser;
using Reax.Parser.Node;
using Reax.Core.Ast.Expressions;
using Reax.Core.Ast.Literals;
using Reax.Core.Ast.Statements;
using Reax.Semantic.Contexts;
using Reax.Core.Ast;
using Reax.Core.Ast.Objects;
using Reax.Core.Ast.Interfaces;
using Reax.Core.Ast.Objects.Structs;

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
        Handlers[typeof(ForInNode)] = ApplyForInNode;
        Handlers[typeof(ArrayNode)] = ApplyArrayNode;
        Handlers[typeof(StructInstanceNode)] = ApplyStructInstanceNode;
    }

    private ValidationResult ApplyStructInstanceNode(IReaxNode node)
    {
        var instance = (StructInstanceNode)node;
        var result = ValidationResult.Success();

        var symbol = Context.Resolve(instance.Name);
        if (symbol is null)
            result.Join(ValidationResult.FailureSymbolUndeclared(instance.Name, instance.Location));

        result.Join(ApplyStructInstanceNodeValidateProperties(instance));
        return result;

    }

    private ValidationResult ApplyStructInstanceNodeValidateProperties(
        StructInstanceNode instance)
    {
        var result = ValidationResult.Success();
        var symbolProperties = Context.ResolveChildren(instance.Name);
        foreach (var value in instance.FieldValues)
        {
            var symbolProperty = symbolProperties.FirstOrDefault(x => x.Identifier == value.Key);
            if (symbolProperty is null)
            {
                result.Join(
                    ValidationResult.FailureSymbolUndeclared(
                        $"{instance.Name}.{value.Key}",
                        instance.Location));

                continue;
            }

            var expectedType = symbolProperty.Type;
            var currentType = GetDataType(value.Value);
            if (expectedType != currentType)
                result.Join(
                    ValidationResult.FailureIncompatibleTypes(
                        expectedType,
                        currentType,
                        value.Value.Location));

        }

        return result;
    }

    private ValidationResult ApplyAssignmentNode(IReaxNode node)
    {
        var assignment = (AssignmentNode)node;
        var expected = GetDataType(assignment.Identifier);
        var current = GetDataType(assignment.Assigned);

        if (current.HasFlag(expected) && assignment.Assigned is ExpressionNode)
            return ValidationResult.Success();
        else if (expected == current && assignment.Assigned is ObjectNode or LiteralNode)
            return ValidationResult.Success();
        else
            return ValidationResult.FailureIncompatibleTypes(expected, current, assignment.Location);
    }

    private ValidationResult ApplyActionNode(IReaxNode node)
    {
        var action = (ActionNode)node;
        var expected = action.Type;
        var current = GetDataType(action.Context);
        if (current.HasFlag(expected))
            return ValidationResult.Success();
        else
            return ValidationResult.FailureIncompatibleTypes(expected, current, action.Location);
    }

    private ValidationResult ApplyFunctionDeclarationNode(IReaxNode node)
    {
        var declarationNode = (FunctionDeclarationNode)node;
        var expected = declarationNode.SuccessType | declarationNode.ErrorType;
        var current = GetDataType(declarationNode.Block);
        if ((expected & current) == current)
            return ValidationResult.Success();
        else
            return ValidationResult.FailureIncompatibleTypes(declarationNode.SuccessType | declarationNode.ErrorType, current, declarationNode.Location);
    }

    private ValidationResult ApplyFunctionCallNode(IReaxNode node)
    {
        var call = (FunctionCallNode)node;
        var symbol = Context.Resolve(call.Identifier);
        if (symbol is null)
            return ValidationResult.FailureSymbolUndeclared(call.Identifier, call.Location);

        var expectedParameters = Context.ResolveChildren(call.Identifier);
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
            return ValidationResult.FailureSymbolUndeclared(call.Identifier, call.Location);

        var expectedParameters = Context.ResolveChildren(call.Identifier, external.scriptName);
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
        if (passedParameters.Length > expectedParameters.Length || passedParameters.Length < requiredParameters)
            return ValidationResult.FailureInvalidFunctionCall_ParametersCount(
                $"{script}.{identifier}",
                expectedParameters.Length,
                passedParameters.Length,
                location);

        for (int i = 0; i < passedParameters.Length; i++)
        {
            DataType passedType = GetDataType(passedParameters[i]);
            if (!expectedParameters[i].Type.IsCompatatible(passedType))
                return ValidationResult.FailureInvalidFunctionCall_InvalidParameter(
                    $"{script}.{identifier}",
                    expectedParameters[i].Identifier,
                    expectedParameters[i].Type,
                    passedType,
                    location);
        }

        return ValidationResult.Success();
    }

    private ValidationResult ApplyForInNode(IReaxNode node)
    {
        var forIn = (ForInNode)node;
        var typeDeclaration = forIn.Declaration.Type | DataType.ARRAY;
        var typeItemArray = GetDataType(forIn.Array);

        if (typeDeclaration == typeItemArray)
            return ValidationResult.Success();
        else
            return ValidationResult.FailureIncompatibleTypes(
                typeDeclaration, typeItemArray, forIn.Location);
    }

    private ValidationResult ApplyArrayNode(IReaxNode node)
    {
        var array = (ArrayNode)node;
        DataType expected = GetDataType(array.FirstOrDefault(new NullNode(array.Location)));

        var result = ValidationResult.Success();
        foreach (var item in array)
        {
            var current = GetDataType(item);
            if (expected != current)
                result.Join(ValidationResult.FailureIncompatibleTypes(
                    expected, current, item.Location));
        }

        return result;
    }

    private DataType GetDataType(ReaxNode node)
    {
        if (node is BinaryNode)
            return DataType.BOOLEAN;
        else if (node is CalculateNode)
            return DataType.NUMBER;
        else if (node is ExternalFunctionCallNode externalFunction)
            return GetDataTypeByIdentifier(externalFunction.functionCall.Identifier, externalFunction.scriptName);
        else if (node is FunctionCallNode functionCall)
            return GetDataTypeByIdentifier(functionCall.Identifier);
        else if (node is VarNode var)
            return var.Type != DataType.NONE ? var.Type : GetDataTypeByIdentifier(var.Identifier);
        else if (node is LiteralNode literal)
            return literal.Type;
        else if (node is MatchNode match)
            return GetResultDataType(match);
        else if (node is ContextNode contextNode)
            return GetDataByContextNode(contextNode);
        else if (node is ReturnSuccessNode successNode)
            return GetDataTypeByReturn(successNode);
        else if (node is ReturnErrorNode errorNode)
            return GetDataTypeByReturn(errorNode);
        else if (node is IfNode ifNode)
            return GetDataTypeByIf(ifNode);
        else if (node is ArrayNode arrayNode)
            return GetDataTypeByArray(arrayNode);
        else if (node is ArrayAccessNode arrayItem)
            return GetDataTypeByArrayItem(arrayItem);
        else if (node is StructInstanceNode)
            return DataType.STRUCT;
        else if (node is StructFieldAccessNode fieldAccessNode)
            return GetDataTypeByProperty(fieldAccessNode);
        else if (node is NativeCallNode native)
            return native.Type;
        else
            return DataType.NONE;
    }

    private DataType GetDataTypeByProperty(StructFieldAccessNode fieldAccessNode)
    {
        var symbol = Context.Resolve(fieldAccessNode.Identifier);
        if (symbol is null || symbol.ParentIdentifier is null)
            return DataType.NONE;

        var properties = Context.ResolveChildren(symbol.ParentIdentifier);
        var property = properties.FirstOrDefault(x => x.Identifier == fieldAccessNode.Property && x.Category == SymbolCategory.PROPERTY);
        if (property is null)
            return DataType.NONE;
        else
            return property.Type;
    }

    private DataType GetDataTypeByIdentifier(string identifier, string? script = null)
    {
        var symbol = Context.Resolve(identifier, script);
        if (symbol is not null)
            return symbol.Type;
        else
            return DataType.NONE;
    }

    private DataType GetResultDataType(MatchNode node)
    {
        if (node.Success.Type == node.Error.Type)
            return node.Success.Type;

        return DataType.NONE;
    }

    private DataType GetDataByContextNode(ContextNode node)
    {
        DataType returnType = DataType.NONE;
        foreach (var item in node.Block)
        {
            var type = (DataType)GetDataType(item);
            if (type != DataType.NONE)
            {
                if (returnType is DataType.NONE or DataType.VOID)
                    returnType = type;
                else
                    returnType = returnType | type;
            }
        }

        return returnType;
    }

    private DataType GetDataTypeByReturn(ReturnSuccessNode successNode)
    {
        if (successNode.Expression is ContextNode context)
            return GetDataByContextNode(context);
        else if (successNode.Expression is VarNode var)
            return GetDataType(var);
        else if (successNode.Expression is LiteralNode literal)
            return literal.Type;
        else if (successNode.Expression is ObjectNode obj)
            return GetDataType(obj);
        else
            return DataType.NONE;
    }

    private DataType GetDataTypeByReturn(ReturnErrorNode successNode)
    {
        if (successNode.Expression is ContextNode context)
            return GetDataByContextNode(context);
        else if (successNode.Expression is VarNode var)
            return GetDataType(var);
        else if (successNode.Expression is LiteralNode literal)
            return literal.Type;
        else
            return DataType.NONE;
    }

    private DataType GetDataTypeByIf(IfNode node)
    {
        var type = GetDataType(node.True);
        if (node.False is null)
            return type;

        type = type | GetDataType(node.False);
        return type;
    }

    private DataType GetDataTypeByArray(ArrayNode array)
    {
        if (array.Literals.Length == 0)
            return DataType.NONE;

        ReaxNode node = array.Literals.First();
        return GetDataType(node) | DataType.ARRAY;
    }

    private DataType GetDataTypeByArrayItem(ArrayAccessNode arrayAccessNode)
    {
        var expectedType = GetDataType(arrayAccessNode.Array);
        if (expectedType.HasFlag(DataType.STRING))
            return DataType.STRING;
        else if (expectedType.HasFlag(DataType.NUMBER))
            return DataType.NUMBER;
        else
            return DataType.NONE;
    }

}
