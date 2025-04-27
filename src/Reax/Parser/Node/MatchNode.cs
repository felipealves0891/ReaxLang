using System;
using Reax.Lexer;

namespace Reax.Parser.Node;

public record MatchNode(
    ReaxNode Expression,    
    ActionNode Success,
    ActionNode Error,  
    SourceLocation Location) : ReaxNode(Location)
{
    public override string ToString()
    {
        return $"match {Expression} {{ success, error }}";
    }
}
