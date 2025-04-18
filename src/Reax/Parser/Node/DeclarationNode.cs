namespace Reax.Parser.Node;

public record DeclarationNode(string Identifier, bool immutable, ReaxNode? Assignment) : ReaxNode
{
    public override string ToString()
    {
        var mut = immutable ? "const" : "let";
        if(Assignment is not null)
            return $"{mut} {Identifier} = {Assignment};";
        else 
            return $"{mut} {Identifier};";
    }
}
