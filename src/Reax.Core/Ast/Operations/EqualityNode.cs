using Reax.Core.Locations;
using Reax.Core.Ast.Interfaces;

namespace Reax.Core.Ast.Operations;

public record EqualityNode(
    string Operator, 
    SourceLocation Location) : ReaxNode(Location), ILogicOperator
{
    public override IReaxNode[] Children => [];

    public bool Compare(ReaxNode x, ReaxNode y)
    {
        if(x is IReaxValue xValue && y is IReaxValue yValue)
            return Compare(xValue, yValue);
        else
            throw new InvalidOperationException("Equality esperava dois valores para comparar");
    }

    private bool Compare(IReaxValue x, IReaxValue y)
    {
        if(Operator == "==" )
            return x.Value.Equals(y.Value);
        else
            return !x.Value.Equals(y.Value);
    }

    public override void Serialize(BinaryWriter writer)
    {
        var typename = GetType().AssemblyQualifiedName
            ?? throw new InvalidOperationException("Tipo nulo ao serializar");

        writer.Write(typename);

        writer.Write(Operator);
        base.Serialize(writer);
    }

    public static new EqualityNode Deserialize(BinaryReader reader)
    {
        var op = reader.ReadString();
        var location = ReaxNode.Deserialize(reader);
        return new EqualityNode(op, location);
    }
    
    public override string ToString()
    {
        return Operator.ToString();
    }
}
