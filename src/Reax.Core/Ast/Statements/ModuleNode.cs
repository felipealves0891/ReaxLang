using System.Diagnostics.CodeAnalysis;
using Reax.Core.Functions;
using Reax.Core.Locations;


namespace Reax.Core.Ast.Statements;

[ExcludeFromCodeCoverage]
public record ModuleNode(
    string identifier, 
    Dictionary<string, Function> functions, 
    SourceLocation Location) : StatementNode(Location), IReaxDeclaration
{
    public override IReaxNode[] Children => [];

    public override void Execute(IReaxExecutionContext context)
    {
        context.Declare(identifier);
        context.SetModule(identifier, functions);  
    }

    public override string ToString()
    {
        return $"import module {identifier};";
    }
}
