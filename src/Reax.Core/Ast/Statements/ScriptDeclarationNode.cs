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

    public override void Serialize(BinaryWriter writer)
    {
        var typename = GetType().AssemblyQualifiedName
            ?? throw new InvalidOperationException("Tipo nulo ao serializar");

        writer.Write(typename);

        writer.Write(Identifier);
        base.Serialize(writer);
    }

    public static new ScriptDeclarationNode Deserialize(BinaryReader reader)
    {
        var identifier = reader.ReadString();
        var location = ReaxNode.Deserialize(reader);
        return new ScriptDeclarationNode(identifier, location);
    }

    public override string ToString()
    {
        return $"script {Identifier};";
    }
}
