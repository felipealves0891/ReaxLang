using Reax.Parser.Node.Interfaces;
using Reax.Runtime;
using Reax.Semantic.Analyzers.TypeChecking;
using Reax.Semantic.Node;

namespace Reax.Parser.Node;

public record VarNode(
    string Identifier, 
    DataType Type,
    SourceLocation Location) : ReaxNode(Location), IReaxValue, INodeResultType
{
    public object Value => Identifier;
    public DataType ResultType => Type;
    public bool IsLeaf => true;
    public INode[] Children => [];

    public override string ToString()
    {
        return Identifier;
    }
}
