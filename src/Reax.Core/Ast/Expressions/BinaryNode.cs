using System.Diagnostics.CodeAnalysis;
using Reax.Core.Ast.Interfaces;
using Reax.Core.Ast.Literals;
using Reax.Core.Locations;

namespace Reax.Core.Ast.Expressions;

[ExcludeFromCodeCoverage]
public record BinaryNode(
    ReaxNode Left, 
    ReaxNode Operator, 
    ReaxNode Right, 
    SourceLocation Location) : ExpressionNode(Location)
{
    public override IReaxNode[] Children => [Left, Operator, Right];

    public override LiteralNode Evaluation(IReaxExecutionContext context)
    {
        var left = Left is BinaryNode 
                 ? new BooleanNode(((BinaryNode)Left).Evaluation(context).ToString(), Location) 
                 : Left.GetValue(context);

        var right = Right is BinaryNode 
                 ? new BooleanNode(((BinaryNode)Right).ToString(), Location) 
                 : Right.GetValue(context);

        var logical = (ILogicOperator)Operator;
        return new BooleanNode(logical.Compare(left, right).ToString(), Operator.Location);
    }

    public override string ToString()
    {
        return $"{Left} {Operator} {Right}";
    }
}
