using System;
using System.Diagnostics.CodeAnalysis;
using Reax.Core.Locations;

namespace Reax.Parser.Node.Statements;

[ExcludeFromCodeCoverage]
public abstract record StatementNode(SourceLocation Location) 
    : ReaxNode(Location)
{}
