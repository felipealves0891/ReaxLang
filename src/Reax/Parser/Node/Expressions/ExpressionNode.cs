using System;
using Reax.Semantic;

namespace Reax.Parser.Node.Expressions;

public abstract record ExpressionNode(SourceLocation Location)
    : ReaxNode(Location)
{
}
