using System;
using Reax.Core.Ast.Expressions;
using Reax.Core.Ast.Interfaces;
using Reax.Core.Locations;

namespace Reax.Core.Ast.Objects.Structs;

public record StructFieldAccessNode(
    string Identifier,
    string Property,
    SourceLocation Location)
    : ExpressionNode(Location)
{
    public override IReaxNode[] Children => [];

    public override IReaxValue Evaluation(IReaxExecutionContext context)
    {
        var variable = context.GetVariable(Identifier);
        var obj = (StructInstanceNode)variable;
        return obj.FieldValues[Property].GetValue(context);
    }

    public override string ToString()
    {
        return $"{Identifier}->{Property}";
    }
}
