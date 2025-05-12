using System.Diagnostics.CodeAnalysis;
using Reax.Core.Locations;
using Reax.Core.Types;
using Reax.Core.Ast.Interfaces;

namespace Reax.Core.Ast.Expressions;

[ExcludeFromCodeCoverage]
public record VarNode : ExpressionNode, IReaxValue
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
    
    public override string ToString()
    {
        return Identifier;
    }
}
