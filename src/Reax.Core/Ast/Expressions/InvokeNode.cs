using System;
using Reax.Core.Ast.Interfaces;
using Reax.Core.Ast.Statements;
using Reax.Core.Helpers;
using Reax.Core.Locations;
using Reax.Parser.Node;

namespace Reax.Core.Ast.Expressions;

public record InvokeNode(
    string Identifier,
    SourceLocation Location) : ExpressionNode(Location)
{
    public override IReaxNode[] Children => [];

    public override IReaxValue Evaluation(IReaxExecutionContext context)
    {
        var invokable = context.GetVariable(Identifier).Value;

        if (invokable is ExpressionNode expression)
            return expression.Evaluation(context);

        if (invokable is IReaxValue value)
            return value;

        if (invokable is StatementNode statement)
            statement.Execute(context);

        return new NullNode(Location);
    }

    public override void Serialize(BinaryWriter writer)
    {
        var typename = GetType().AssemblyQualifiedName
            ?? throw new InvalidOperationException("Tipo nulo ao serializar");

        writer.Write(typename);
        writer.Write(Identifier);
        base.Serialize(writer);
    }

    public static new InvokeNode Deserialize(BinaryReader reader)
    {
        var identifier = reader.ReadString();
        var location = ReaxNode.Deserialize(reader);
        return new InvokeNode(identifier, location);
    }


    public override string ToString()
    {
        return $"invoke {Identifier};";
    }
}
