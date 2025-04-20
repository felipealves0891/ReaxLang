using System;
using Reax.Parser;
using Reax.Parser.Node;
using Reax.Parser.Node.Interfaces;
using Reax.Runtime;

namespace Reax.Semantic;

public class SemanticTypeAnalyzer
{
    public void TypeAnalyzer(IEnumerable<ReaxNode> nodes) 
    {
        foreach (var node in nodes)
        {
            if(node is DeclarationNode declaration)
            {
                if(declaration.Assignment is AssignmentNode assignment)
                    TypeAnalyzer([assignment]);
                else if(declaration.Assignment is IReaxResultType value)
                    ValidateValueType(declaration.Identifier, value, declaration.Location);
                else if(declaration.Assignment is null)
                    continue;
                else
                    throw new InvalidOperationException($"{declaration.Location} - Declaração sem atribuição!");
            }
            else if(node is AssignmentNode assignment)
            {
                ValidateAssignmentType(assignment);
            }
        }
    }
    
    public void ValidateAssignmentType(AssignmentNode assignment)
    {
        var identifier = assignment.Identifier;
        var symbol = ReaxEnvironment.Symbols[identifier];

        if(assignment.Assigned is IReaxResultType resultType)
        {
            if(!resultType.GetDataType().IsCompatible(symbol.Type))    
                throw new InvalidOperationException($"{assignment.Location.File}({assignment.Location.Line}) Atribuição de tipo invalido: Esperado {symbol.Type}, mas passado {resultType.GetDataType()}!");
        }
        else
        {
            throw new InvalidOperationException($"{assignment.Location.File}({assignment.Location.Line}) Atribuição de tipo invalido: Esperado {symbol.Type}, mas passado não returna um tipo!");
        }

    }

    public void ValidateValueType(string identifier, IReaxResultType value, SourceLocation location)
    {
        var symbol = ReaxEnvironment.Symbols[identifier];
        if(!value.GetDataType().IsCompatible(symbol.Type))    
                throw new InvalidOperationException($"{location} - Atribuição de tipo invalido: Esperado {symbol.Type}, mas passado {value.GetDataType()}!");
    }

}
