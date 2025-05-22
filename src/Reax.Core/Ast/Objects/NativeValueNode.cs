using System;
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
}
