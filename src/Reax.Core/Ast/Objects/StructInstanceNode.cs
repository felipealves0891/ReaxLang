using System;
using Reax.Core.Ast.Expressions;
using Reax.Core.Ast.Interfaces;
using Reax.Core.Locations;
using Reax.Core.Types;

namespace Reax.Core.Ast.Objects;

public record StructInstanceNode(
    string Name,
    Dictionary<string, ReaxNode> FieldValues,
    SourceLocation Location) : ObjectNode(Location), IReaxValue
{
    public override IReaxNode[] Children => FieldValues.Values.Cast<IReaxNode>().ToArray();
    public object Value => FieldValues;
    public DataType Type => DataType.STRUCT;

    public override string ToString()
    {        
        return $"{Name} {{}}";
    }
}
