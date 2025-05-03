using Reax.Parser.Node;
using Reax.Parser.Node.Statements;
using Reax.Runtime.Functions;
using Reax.Semantic.Contexts;

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
    }

    private ValidationResult ApplyDeclarationNode(IReaxNode node)
    {
        var declaration = (DeclarationNode)node;
        if(declaration.Immutable)
        {
            var symbol = Symbol.CreateConst(declaration.Identifier, declaration.Type, declaration.Location);
            return Context.Declare(symbol);
        }
        
        if(declaration.Async)
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
        if(assignment.Identifier.Type != Parser.DataType.NONE)
            return ValidationResult.Success();

        var symbol = Context.Resolve(assignment.Identifier.Identifier);
        if(symbol is null)
            return ValidationResult.SymbolUndeclared(assignment.Identifier.Identifier, assignment.Location);

        assignment.Identifier.Type = symbol.Type;
        return ValidationResult.Success();
    }

    private ValidationResult ApplyVarNode(IReaxNode node) 
    {
        var variable = (VarNode)node;
        if(variable.Type != Parser.DataType.NONE)
        {
            var declarationSymbol = Symbol.CreateConst(variable.Identifier, variable.Type, variable.Location);
            return Context.Declare(declarationSymbol);
        }
        
        var symbol = Context.Resolve(variable.Identifier);
        if(symbol is null)
            return ValidationResult.SymbolUndeclared(variable.Identifier, variable.Location);

        variable.Type = symbol.Type;
        return ValidationResult.Success();
    }

    private ValidationResult ApplyActionNode(IReaxNode node)
    {
        var action = (ActionNode)node;
        var results = ValidationResult.Success();
        foreach (var parameter in action.Parameters)
        {
            var symbolDeclaration = Symbol.CreateConst(parameter.Identifier, parameter.Type, parameter.Location);
            results.Join(Context.Declare(symbolDeclaration));
        }
        return results;
    }
    
}
