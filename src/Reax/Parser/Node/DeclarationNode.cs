namespace Reax.Parser.Node;

public record DeclarationNode(string Identifier, ReaxNode? Assignment) : ReaxNode
{
    public override string ToString()
    {
        if(Assignment is not null)
            return $"let {Identifier} = {Assignment};";
        else 
            return $"let {Identifier};";
    }
}
