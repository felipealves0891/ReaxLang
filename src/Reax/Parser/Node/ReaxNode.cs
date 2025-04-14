using System;
using Reax.Runtime;

namespace Reax.Parser.Node;

public interface IReaxValue;
public interface IOperator;
public interface IArithmeticOperator : IOperator
{
    NumberNode Calculate(NumberNode x, NumberNode y);
};
public interface ILogicOperator : IOperator
{
    bool Compare(ReaxNode x, ReaxNode y);
};

public abstract record ReaxNode 
{
    public ReaxNode GetValue(ReaxExecutionContext context) 
    {
        if(this is NumberNode number)
            return number;
        else if(this is StringNode text)
            return text;
        else if(this is VarNode variable)
            return context.GetVariable(variable.Identifier);
        else
            throw new InvalidOperationException("Não foi possivel identificar o tipo da variavel!");
    }
}

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

public record FunctionCallNode(string Identifier, ReaxNode[] Parameter) : ReaxNode
{
    public override string ToString()
    {
        return $"{Identifier}({string.Join(',', Parameter.Select(x => x.ToString()))});";
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
    public NumberNode Calculate(NumberNode x, NumberNode y)
    {
        return Operator switch 
        {
            "+" => new NumberNode((x.ValueConverted + y.ValueConverted).ToString()),
            "-" => new NumberNode((x.ValueConverted - y.ValueConverted).ToString()),
            _ => throw new InvalidOperationException("Operador invalido!")
        };
    }

    public override string ToString()
    {
        return Operator.ToString();
    }
}

public record FactorNode(string Operator) : ReaxNode, IArithmeticOperator
{
    public NumberNode Calculate(NumberNode x, NumberNode y)
    {
        return Operator switch 
        {
            "*" => new NumberNode((x.ValueConverted * y.ValueConverted).ToString()),
            "/" => new NumberNode((x.ValueConverted / y.ValueConverted).ToString()),
            _ => throw new InvalidOperationException("Operador invalido!")
        };
    }

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

public record IfNode(BinaryNode Condition, ReaxNode True, ReaxNode? False) : ReaxNode
{
    public override string ToString()
    {
        var elseText = False is null ? "" : "else {}";
        return $"if {Condition} {{}} {elseText}";
    }
}

public record ObservableNode(ReaxNode Var, ReaxNode Block, BinaryNode? Condition) : ReaxNode
{
    public override string ToString()
    {
        return $"Observer {Var} {{}}";
    }
}

public record FunctionNode(ReaxNode Identifier, ReaxNode Block, ReaxNode[] Parameters) : ReaxNode
{
    public override string ToString()
    {
        var param = string.Join(',', Parameters.Select(x => x.ToString()));
        return $"fun {Identifier} ({param}) {{...}}";
    }
}

public record ReturnNode(ReaxNode Expression) : ReaxNode
{
    public override string ToString()
    {
        return $"return {Expression}";
    }
}