using System;

namespace Reax.Parser.Node;

public interface ReaxValue;

public record ReaxNode;

public record StringNode(string Value) : ReaxNode, ReaxValue
{
    public override string ToString()
    {
        return $"'{Value}'";
    }
}

public record NumberNode(string Value) : ReaxNode, ReaxValue
{
    public override string ToString()
    {
        return $"{Value}";
    }
}

public record VarNode(string Identifier) : ReaxNode, ReaxValue
{
    public override string ToString()
    {
        return Identifier;
    }
}

public record FunctionCallNode(string Identifier, ReaxNode Parameter) : ReaxNode
{
    public override string ToString()
    {
        return $"{Identifier}({Parameter});";
    }
}

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

public record AssignmentNode(string Identifier, ReaxNode Assignment) : ReaxNode
{
    public override string ToString()
    {
        return $"{Identifier} = {Assignment};";
    }
}
