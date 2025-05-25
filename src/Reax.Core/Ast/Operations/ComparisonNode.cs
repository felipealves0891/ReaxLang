using Reax.Core.Locations;
using Reax.Core.Ast.Interfaces;
using Reax.Core.Ast.Literals;

namespace Reax.Core.Ast.Operations;

public record ComparisonNode(
    string Operator, 
    SourceLocation Location) : ReaxNode(Location), ILogicOperator
{
    public override IReaxNode[] Children => [];

    public bool Compare(ReaxNode x, ReaxNode y)
    {
        var left = Convert.ToDecimal(((NumberNode)x).Value);
        var rigth = Convert.ToDecimal(((NumberNode)y).Value);

        if(Operator == "<")
            return left < rigth;
        else if(Operator == ">")
            return left > rigth;
        else if(Operator == "<=")
            return left <= rigth;
        else if(Operator == ">=")
            return left > rigth;
        else
            throw new InvalidOperationException($"Operador de comparação invalido: {Operator}");
    }

    public override void Serialize(BinaryWriter writer)
    {
        var typename = GetType().AssemblyQualifiedName
            ?? throw new InvalidOperationException("Tipo nulo ao serializar");

        writer.Write(typename);

        writer.Write(Operator);
        base.Serialize(writer);
    }

    public static new ComparisonNode Deserialize(BinaryReader reader)
    {
        var op = reader.ReadString();
        var location = ReaxNode.Deserialize(reader);
        return new ComparisonNode(op, location);
    }

    public override string ToString()
    {
        return Operator.ToString();
    }
}
