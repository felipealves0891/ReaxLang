using Reax.Core.Locations;
using Reax.Core.Ast.Interfaces;
using Reax.Core.Ast.Literals;

namespace Reax.Core.Ast.Operations;

public record LogicNode(string Operator, 
    SourceLocation Location) : ReaxNode(Location), ILogicOperator
{
    public override IReaxNode[] Children => [];

    public bool Compare(ReaxNode x, ReaxNode y)
    {
        var left = (BooleanNode)x;
        var right = (BooleanNode)y;

        return Operator switch 
        {
            "and" => (bool)left.Value && (bool)right.Value,
            "or" => (bool)left.Value || (bool)right.Value,
            _ => throw new InvalidOperationException($"Operador invalido para operação logica {Operator}!")
        };
    }

    public override void Serialize(BinaryWriter writer)
    {
        var typename = GetType().AssemblyQualifiedName
            ?? throw new InvalidOperationException("Tipo nulo ao serializar");

        writer.Write(typename);

        writer.Write(Operator);
        base.Serialize(writer);
    }

    public static new LogicNode Deserialize(BinaryReader reader)
    {
        var op = reader.ReadString();
        var location = ReaxNode.Deserialize(reader);
        return new LogicNode(op, location);
    }

    public override string ToString()
    {
        return Operator.ToString();
    }
}
