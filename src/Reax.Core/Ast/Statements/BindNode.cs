using System;
using System.Diagnostics.CodeAnalysis;
using Reax.Core.Locations;
using Reax.Core.Types;
using Reax.Core.Ast.Interfaces;
using Reax.Core.Ast.Statements;


namespace Reax.Core.Ast.Statements;

[ExcludeFromCodeCoverage]
public record BindNode(
    string Identifier, 
    AssignmentNode Node,
    DataType Type, 
    SourceLocation Location) : StatementNode(Location)
{
    public override IReaxNode[] Children => Node.Assigned is ContextNode context ? context.Block : [Node.Assigned];
    public override string ToString()
    {
        return $"bind {Identifier}: {Type} -> {{...}}";
    }
}
