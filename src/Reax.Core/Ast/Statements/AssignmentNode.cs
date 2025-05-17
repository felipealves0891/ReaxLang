using System.Diagnostics.CodeAnalysis;
using Reax.Core.Locations;
using Reax.Core.Ast.Interfaces;
using Reax.Core.Ast.Expressions;
using Reax.Core.Ast.Literals;
using Reax.Core.Ast.Objects;


namespace Reax.Core.Ast.Statements;

[ExcludeFromCodeCoverage]
public record AssignmentNode(
    VarNode Identifier, 
    ReaxNode Assigned, 
    SourceLocation Location) : StatementNode(Location)
{
    public override IReaxNode[] Children => [Assigned];

    public override void Execute(IReaxExecutionContext context)
    {
        if (Assigned is ExpressionNode expression)
            context.SetVariable(Identifier.Identifier, expression.Evaluation(context));
        else if (Assigned is VarNode var)
            context.SetVariable(Identifier.Identifier, var.Evaluation(context));
        else if (Assigned is IReaxValue value)
            context.SetVariable(Identifier.Identifier, value);
        else
            throw new Exception($"Tipo invalido para direita de uma atribuição {Assigned.GetType().Name}, era esperado uma expressão ou um literal!");
    }

    public override string ToString()
    {
        return $"{Identifier} = {Assigned};";
    }
}
