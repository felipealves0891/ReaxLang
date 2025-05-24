using System;
using Reax.Core.Debugger;
using Reax.Core.Locations;

namespace Reax.Core.Ast.Statements;

public record FreeNode(string Identifier, SourceLocation Location)
    : StatementNode(Location)
{
    public override IReaxNode[] Children => [];

    public override void Execute(IReaxExecutionContext context)
    {
        if (!context.Remove(Identifier))
            Logger.LogRuntime($"Não foi possivel remover o identificador {Identifier}!");
    }

    public override void Serialize(BinaryWriter writer)
    {
        var typename = GetType().AssemblyQualifiedName
            ?? throw new InvalidOperationException("Tipo nulo ao serializar");

        writer.Write(typename);

        writer.Write(Identifier);
        base.Serialize(writer);
    }

    public override string ToString()
    {
        return $"free {Identifier};";
    }
}
