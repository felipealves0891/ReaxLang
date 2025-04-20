using Reax.Runtime;

namespace Reax.Parser.Node;

public abstract record ReaxNode(SourceLocation Location)
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
