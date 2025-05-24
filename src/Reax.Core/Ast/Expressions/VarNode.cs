using System.Diagnostics.CodeAnalysis;
using Reax.Core.Locations;
using Reax.Core.Types;
using Reax.Core.Ast.Interfaces;
using Reax.Core.Ast.Literals;

namespace Reax.Core.Ast.Expressions;

[ExcludeFromCodeCoverage]
public record VarNode : ExpressionNode
{
    public VarNode(
        string identifier, 
        DataType type,
        SourceLocation location) : base(location)
    {
        Identifier = identifier;
        Type = type;
    }

    public object Value => Identifier;
    public override IReaxNode[] Children => [];

    public string Identifier { get; }
    public DataType Type { get; set; }

    public override IReaxValue Evaluation(IReaxExecutionContext context)
    {
        return context.GetVariable(Identifier);
    }

    public override void Serialize(BinaryWriter writer)
    {
        var typename = GetType().AssemblyQualifiedName
            ?? throw new InvalidOperationException("Tipo nulo ao serializar");

        writer.Write(typename);

        writer.Write(Identifier);
        writer.Write((int)Type);
        base.Serialize(writer);
    }
    
    public override string ToString()
    {
        return Identifier;
    }
}
