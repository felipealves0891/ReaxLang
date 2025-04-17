using System;
using Reax.Lexer;
using Reax.Parser.Node;

namespace Reax.Parser.NodeParser;

public interface INodeParser
{
    public bool IsParse(Token before, Token current, Token next);
    public ReaxNode? Parse(ITokenSource source);
}
