using System;
using System.Diagnostics.CodeAnalysis;
using Reax.Core;
using Reax.Core.Ast;
using Reax.Core.Ast.Statements;
using Reax.Core.Locations;


namespace Reax.Parser.Node;

[ExcludeFromCodeCoverage]
public record ScriptDeclarationNode(
    string Identifier, 
    SourceLocation Location) 
    : StatementNode(Location), IReaxDeclaration
{
    public override IReaxNode[] Children => [];

    public override void Execute(IReaxExecutionContext context)
    {
    }

    public override string ToString()
    {
        return $"script {Identifier};";
    }
}
