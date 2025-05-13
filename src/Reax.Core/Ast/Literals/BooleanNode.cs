using System.Diagnostics.CodeAnalysis;
using Reax.Core.Locations;
using Reax.Core.Types;
using Reax.Core.Ast.Interfaces;
using Reax.Core.Ast.Literals;

namespace Reax.Core.Ast.Literals;

[ExcludeFromCodeCoverage]
public record BooleanNode(
    string Source, 
    SourceLocation Location) : LiteralNode(Source, Location)
{
    public override object Value => bool.Parse(Source.ToLower());
    public override DataType Type => DataType.BOOLEAN;
    public override IReaxNode[] Children => [];

    public override string ToString()
    {
        return $"{Source.ToLower()}";
    }
}
