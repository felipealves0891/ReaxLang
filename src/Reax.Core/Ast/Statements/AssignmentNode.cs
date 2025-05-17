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
        else if (Assigned is LiteralNode literal)
            context.SetVariable(Identifier.Identifier, literal);
        else if (Assigned is ObjectNode @object)
            context.SetVariable(Identifier.Identifier, @object);
        else
            throw new Exception("Tipo invalido para direita de uma atribuição, era esperado uma expressão ou um literal!");
    }

    public override string ToString()
    {
        return $"{Identifier} = {Assigned};";
    }
}
