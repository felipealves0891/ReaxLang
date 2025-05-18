using Reax.Core.Locations;
using Reax.Core.Types;
using Reax.Parser;
using Reax.Parser.Node;
using Reax.Core.Ast.Statements;
using Reax.Semantic.Contexts;
using Reax.Core.Ast;
using Reax.Core.Ast.Expressions;
using Reax.Core.Functions;

namespace Reax.Semantic.Rules;

public class SymbolRule : BaseRule
{
    public SymbolRule() : base()
    {
        Handlers[typeof(DeclarationNode)] = ApplyDeclarationNode;
        Handlers[typeof(BindNode)] = ApplyBindDeclarationNode;
        Handlers[typeof(FunctionDeclarationNode)] = ApplyFunctionDeclarationNode;
        Handlers[typeof(AssignmentNode)] = ApplyAssignmentNode;
        Handlers[typeof(VarNode)] = ApplyVarNode;
        Handlers[typeof(ActionNode)] = ApplyActionNode;
        Handlers[typeof(ModuleNode)] = ApplyModuleNode;
        Handlers[typeof(StructDeclarationNode)] = ApplyStructDeclarationNode;
    }

    private ValidationResult ApplyDeclarationNode(IReaxNode node)
    {
        var declaration = (DeclarationNode)node;
        if (declaration.Immutable)
        {
            var symbol = Symbol.CreateConst(declaration.Identifier, declaration.Type, declaration.Location);
            return Context.Declare(symbol);
        }

        if (declaration.Async)
        {
            var symbol = Symbol.CreateLetAsync(declaration.Identifier, declaration.Type, declaration.Location);
            return Context.Declare(symbol);
        }
        else
        {
            var symbol = Symbol.CreateLet(declaration.Identifier, declaration.Type, declaration.Location);
            return Context.Declare(symbol);
        }

    }

    private ValidationResult ApplyBindDeclarationNode(IReaxNode node)
    {
        var declaration = (BindNode)node;
        var symbol = Symbol.CreateBind(declaration.Identifier, declaration.Type, declaration.Location);
        return Context.Declare(symbol);
    }

    private ValidationResult ApplyFunctionDeclarationNode(IReaxNode node)
    {
        var declaration = (FunctionDeclarationNode)node;
        var symbol = Symbol.CreateFunction(declaration.Identifier, declaration.SuccessType | declaration.ErrorType, declaration.Location);

        var result = Context.Declare(symbol);
        foreach (var param in declaration.Parameters)
        {
            var paramSymbol = Symbol.CreateParameter(param.Identifier, declaration.Identifier, param.Type, param.Location);
            result.Join(Context.Declare(paramSymbol));
        }

        return result;
    }

    private ValidationResult ApplyAssignmentNode(IReaxNode node)
    {
        var assignment = (AssignmentNode)node;
        if (assignment.Identifier.Type != DataType.NONE)
            return ValidationResult.Success();

        var symbol = Context.Resolve(assignment.Identifier.Identifier);
        if (symbol is null)
            return ValidationResult.FailureSymbolUndeclared(assignment.Identifier.Identifier, assignment.Location);

        assignment.Identifier.Type = symbol.Type;
        return ValidationResult.Success();
    }

    private ValidationResult ApplyVarNode(IReaxNode node)
    {
        var variable = (VarNode)node;
        if (variable.Type != DataType.NONE)
        {
            var declarationSymbol = Symbol.CreateConst(variable.Identifier, variable.Type, variable.Location);
            return Context.Declare(declarationSymbol);
        }

        var symbol = Context.Resolve(variable.Identifier);
        if (symbol is null)
            return ValidationResult.FailureSymbolUndeclared(variable.Identifier, variable.Location);

        variable.Type = symbol.Type;
        return ValidationResult.Success();
    }

    private ValidationResult ApplyActionNode(IReaxNode node)
    {
        var action = (ActionNode)node;
        var results = ValidationResult.Success();
        var parameter = action.Parameter;
        var symbolDeclaration = Symbol.CreateConst(parameter.Identifier, parameter.Type, parameter.Location);
        results.Join(Context.Declare(symbolDeclaration));
        return results;
    }

    private ValidationResult ApplyModuleNode(IReaxNode node)
    {
        var module = (ModuleNode)node;
        var results = ValidationResult.Success();
        foreach (var item in module.functions)
        {
            var name = item.Key;
            var function = item.Value;
            if (function is DecorateFunctionBuiltIn decorate)
            {
                var symbolFunction = Symbol.CreateFunction(name, decorate.Result, new SourceLocation());
                results.Join(Context.Declare(symbolFunction));

                for (int i = 0; i < decorate.ParametersCount; i++)
                {
                    var parameterType = decorate.Parameters[i];
                    if (i < decorate.RequiredParametersCount)
                    {
                        var symbolParameter = Symbol.CreateParameter($"{name}_parameter_{i}", name, parameterType, new SourceLocation());
                        results.Join(Context.Declare(symbolParameter));
                    }
                    else
                    {
                        var symbolParameter = Symbol.CreateParameterOptional($"{name}_parameter_{i}", name, parameterType, new SourceLocation());
                        results.Join(Context.Declare(symbolParameter));
                    }
                }
            }
        }

        return results;
    }

    private ValidationResult ApplyStructDeclarationNode(IReaxNode node)
    {
        var result = ValidationResult.Success();
        var structDeclaration = (StructDeclarationNode)node;

        var symbol = Symbol.CreateStruct(
            structDeclaration.Name,
            DataType.STRUCT,
            structDeclaration.Location);

        result.Join(Context.Declare(symbol));
        foreach (var property in structDeclaration.Properties)
        {
            var symbolProperty = Symbol.CreateStructProperty(
                property.Identifier,
                structDeclaration.Name,
                property.Type,
                property.Location);

            result.Join(Context.Declare(symbolProperty));
        }

        return result;
    }


    
}
