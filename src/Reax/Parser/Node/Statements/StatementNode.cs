using System;
using System.Diagnostics.CodeAnalysis;

namespace Reax.Parser.Node.Statements;

[ExcludeFromCodeCoverage]
public abstract record StatementNode(SourceLocation Location) 
    : ReaxNode(Location)
{}
