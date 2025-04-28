using System;
using Reax.Parser.Node;
using Reax.Parser.Node.Interfaces;

namespace Reax.Semantic;

public class SemanticAnalyzer
{
    private readonly ISemanticContext _context;
    
    public SemanticAnalyzer()
    {
        _context = new SemanticContext();
    }

    public void Analyze(ReaxNode[] nodes) 
    {
        var results = new List<IValidateResult>();

        using(_context.EnterScope())
        {
            foreach (var node in nodes)
            {
                if(node is IReaxResult result)
                    results.Add(result.Validate(_context));
            }
        }
    }
}
