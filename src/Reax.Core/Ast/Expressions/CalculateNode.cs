using System.Diagnostics.CodeAnalysis;
using Reax.Core.Ast.Interfaces;
using Reax.Core.Ast.Literals;
using Reax.Core.Locations;

namespace Reax.Core.Ast.Expressions;

[ExcludeFromCodeCoverage]
public record CalculateNode(
    ReaxNode Left, 
    ReaxNode Operator, 
    ReaxNode Right, 
    SourceLocation Location) : ExpressionNode(Location)
{
    public override IReaxNode[] Children => [Left, Operator, Right];

    public override LiteralNode Evaluation(IReaxExecutionContext context)
    {
        var op = (IArithmeticOperator)Operator;
        var left = CalculateChild(Left, context) as NumberNode;
        var right = CalculateChild(Right, context) as NumberNode;

        if(left is null)
            throw new InvalidOperationException($"Valor invalido para calculo '{left}'");
        
        if(right is null)
            throw new InvalidOperationException($"Valor invalido para calculo '{right}'");

        return op.Calculate(left, right);
    }

    private IReaxValue CalculateChild(ReaxNode node, IReaxExecutionContext context) 
    {
        if (node is ExpressionNode expression)
            return expression.Evaluation(context);
        else if (node is LiteralNode literal)
            return literal;
        else
            throw new InvalidOperationException("Não é possivel tratar o nó da operação!");
    }

    public override string ToString()
    {
        return $"{Left} {Operator} {Right}";
    }
}
