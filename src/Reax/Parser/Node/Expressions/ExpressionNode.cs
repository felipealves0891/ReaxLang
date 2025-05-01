using System;

namespace Reax.Parser.Node.Expressions;

public record ExpressionNode(SourceLocation Location) 
    : ReaxNode(Location)
{}
