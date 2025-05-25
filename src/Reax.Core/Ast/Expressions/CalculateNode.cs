using System.Diagnostics.CodeAnalysis;
using Reax.Core.Ast.Interfaces;
using Reax.Core.Ast.Literals;
using Reax.Core.Helpers;
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

    public override void Serialize(BinaryWriter writer)
    {
        var typename = GetType().AssemblyQualifiedName
            ?? throw new InvalidOperationException("Tipo nulo ao serializar");

        writer.Write(typename);

        Left.Serialize(writer);
        Operator.Serialize(writer);
        Right.Serialize(writer);
        base.Serialize(writer);
    }

    public static new CalculateNode Deserialize(BinaryReader reader)
    {
        var left = BinaryDeserializerHelper.Deserialize<ReaxNode>(reader);
        var op = BinaryDeserializerHelper.Deserialize<ReaxNode>(reader);
        var right = BinaryDeserializerHelper.Deserialize<ReaxNode>(reader);
        var location = ReaxNode.Deserialize(reader);
        return new CalculateNode(left, op, right, location);
    }

    public override string ToString()
    {
        return $"{Left} {Operator} {Right}";
    }
}
