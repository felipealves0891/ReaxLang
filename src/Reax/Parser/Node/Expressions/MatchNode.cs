using System;
using Reax.Lexer;
using Reax.Parser.Node.Interfaces;
using Reax.Parser.Node.Statements;

namespace Reax.Parser.Node.Expressions;

public record MatchNode(
    ReaxNode Expression,    
    ActionNode Success,
    ActionNode Error,  
    SourceLocation Location) : ExpressionNode(Location)
{
    public override string ToString()
    {
        return $"match {Expression} {{ success, error }}";
    }
}
