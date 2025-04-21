using Reax.Parser.Node.Interfaces;

namespace Reax.Parser.Node;

public record IfNode(
    BinaryNode Condition, 
    ReaxNode True, 
    ReaxNode? False, 
    SourceLocation Location) : ReaxNode(Location), IReaxContext
{
    public ReaxNode[] Nodes
    {
        get
        {
            var nodes = new List<ReaxNode>();

            if(True is IReaxContext contextTrue)
                nodes.AddRange(contextTrue.Nodes);
            else
                nodes.Add(True);
            
            if(False is IReaxContext contextFalse)
                nodes.AddRange(contextFalse.Nodes);
            else if(False is not null)
                nodes.Add(False);

            return nodes.ToArray();
        }
    }

    public override string ToString()
    {
        var elseText = False is null ? "" : "else {}";
        return $"if {Condition} {{}} {elseText}";
    }
}
