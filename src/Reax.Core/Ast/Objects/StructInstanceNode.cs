using System;
using Reax.Core.Ast.Expressions;
using Reax.Core.Ast.Interfaces;
using Reax.Core.Locations;
using Reax.Core.Types;

namespace Reax.Core.Ast.Objects;

public record StructInstanceNode(
    string Name,
    Dictionary<string, ReaxNode> FieldValues,
    SourceLocation Location) : ObjectNode(Location)
{
    public override IReaxNode[] Children => FieldValues.Values.Cast<IReaxNode>().ToArray();
    public override object Value => FieldValues;
    public override DataType Type => DataType.STRUCT;

    public override string ToString()
    {        
        return $"{Name} {{}}";
    }
}
