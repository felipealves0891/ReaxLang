namespace Reax.Parser.Node;

public record ForNode(ReaxNode declaration, ReaxNode condition, ReaxNode Block) : ReaxNode;
