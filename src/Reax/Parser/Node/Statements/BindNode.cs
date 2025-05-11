using System;
using System.Diagnostics.CodeAnalysis;
using Reax.Core.Locations;
using Reax.Core.Types;
using Reax.Parser.Node.Interfaces;
using Reax.Parser.Node.Statements;
using Reax.Semantic;

namespace Reax.Parser.Node.Statements;

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
