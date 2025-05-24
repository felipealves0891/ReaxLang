using System;
using System.Text.Json;
using Reax.Core.Ast.Interfaces;
using Reax.Core.Locations;
using Reax.Core.Types;

namespace Reax.Core.Ast.Objects;

public record NativeValueNode : ReaxNode, IReaxValue
{
    public NativeValueNode(object value)
        : base(new SourceLocation())
    {
        Value = value;
    }

    public override IReaxNode[] Children => [];

    public object Value { get; }

    public DataType Type => DataType.NONE;

    public override void Serialize(BinaryWriter writer)
    {
        var typename = GetType().AssemblyQualifiedName
            ?? throw new InvalidOperationException("Tipo nulo ao serializar");

        writer.Write(typename);

        var str = Value?.ToString() ?? string.Empty;
        writer.Write(str);
        base.Serialize(writer);
    }

    public override string ToString()
    {
        return JsonSerializer.Serialize(Value);
    }
}
