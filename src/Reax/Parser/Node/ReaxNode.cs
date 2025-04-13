using System;

namespace Reax.Parser.Node;

public interface IReaxValue;
public interface IOperator;
public interface IArithmeticOperator : IOperator;
public interface ILogicOperator : IOperator
{
    bool Compare(ReaxNode x, ReaxNode y);
};
public record ReaxNode;

public record StringNode(string Value) : ReaxNode, IReaxValue
{
    public override string ToString()
    {
        return $"'{Value}'";
    }
}

public record NumberNode(string Value) : ReaxNode, IReaxValue
{
    public decimal ValueConverted => decimal.Parse(Value);

    public override string ToString()
    {
        return $"{Value}";
    }
}

public record VarNode(string Identifier) : ReaxNode, IReaxValue
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

public record ComparisonNode(string Operator) : ReaxNode, ILogicOperator
{
    public bool Compare(ReaxNode x, ReaxNode y)
    {
        var left = (NumberNode)x;
        var rigth = (NumberNode)y;

        if(Operator == "<")
            return left.ValueConverted < rigth.ValueConverted;
        else if(Operator == ">")
            return left.ValueConverted > rigth.ValueConverted;
        else if(Operator == "<=")
            return left.ValueConverted <= rigth.ValueConverted;
        else if(Operator == ">=")
            return left.ValueConverted > rigth.ValueConverted;
        else
            throw new InvalidOperationException("Operador de comparação invalido");
    }

    public override string ToString()
    {
        return Operator.ToString();
    }
}

public record EqualityNode(string Operator) : ReaxNode, ILogicOperator
{
    public bool Compare(ReaxNode x, ReaxNode y)
    {
        return Operator == "==" 
             ? x.ToString() == y.ToString()
             : x.ToString() != y.ToString();
    }

    public override string ToString()
    {
        return Operator.ToString();
    }
}

public record TermNode(string Operator) : ReaxNode, IArithmeticOperator
{
    public override string ToString()
    {
        return Operator.ToString();
    }
}

public record FactorNode(string Operator) : ReaxNode, IArithmeticOperator
{
    public override string ToString()
    {
        return Operator.ToString();
    }
}

public record BinaryNode(ReaxNode Left, ReaxNode Operator, ReaxNode Right) : ReaxNode
{
    public override string ToString()
    {
        return $"{Left} {Operator} {Right}";
    }
}

public record CalculateNode(ReaxNode Left, ReaxNode Operator, ReaxNode Right) : ReaxNode
{
    public override string ToString()
    {
        return $"{Left} {Operator} {Right}";
    }
}

public record ContextNode(ReaxNode[] Block) : ReaxNode;

public record IfNode(BinaryNode Condition, ReaxNode True, ReaxNode? False) : ReaxNode;