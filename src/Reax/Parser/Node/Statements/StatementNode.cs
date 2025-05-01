using System;

namespace Reax.Parser.Node.Statements;

public abstract record StatementNode(SourceLocation Location) 
    : ReaxNode(Location)
{}
