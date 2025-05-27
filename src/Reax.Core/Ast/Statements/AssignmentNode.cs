using System.Diagnostics.CodeAnalysis;
using Reax.Core.Locations;
using Reax.Core.Ast.Interfaces;
using Reax.Core.Ast.Expressions;
using Reax.Core.Ast.Literals;
using Reax.Core.Ast.Objects;
using Reax.Core.Helpers;


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

    public override void Serialize(BinaryWriter writer)
    {
        var typename = GetType().AssemblyQualifiedName
            ?? throw new InvalidOperationException("Tipo nulo ao serializar");

        writer.Write(typename);
        Identifier.Serialize(writer);
        Assigned.Serialize(writer);
        base.Serialize(writer);
    }

    public static new AssignmentNode Deserialize(BinaryReader reader)
    {
        var identifier = BinaryDeserializerHelper.Deserialize<VarNode>(reader);
        var assigned = BinaryDeserializerHelper.Deserialize<ReaxNode>(reader);
        var location = ReaxNode.Deserialize(reader);
        return new AssignmentNode(identifier, assigned, location);
    }

    public override string ToString()
    {
        return $"{Identifier} = {Assigned};";
    }
}
