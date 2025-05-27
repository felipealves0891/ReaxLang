using System;
using Reax.Core;
using Reax.Core.Ast;
using Reax.Core.Ast.Statements;
using Reax.Core.Locations;

namespace Reax.Interpreter.Cache;

public record FileRef(
    string Filename,
    string Identifier,
    SourceLocation Location)
    : ReaxNode(Location)
{
    public override IReaxNode[] Children => [];

    public override void Serialize(BinaryWriter writer)
    {
        var typename = GetType().AssemblyQualifiedName
            ?? throw new InvalidOperationException("Tipo nulo ao serializar");

        writer.Write(typename);
        writer.Write(Filename);
        writer.Write(Identifier);
        base.Serialize(writer);
    }

    public static new ScriptNode Deserialize(BinaryReader reader)
    {
        var filename = reader.ReadString();
        var identifier = reader.ReadString();
        var fullname = Path.Combine(ReaxEnvironment.DirectoryRoot, filename);
        var nodes = ReaxCompiler.GetNodes(fullname);
        var location = ReaxNode.Deserialize(reader);
        return new ScriptNode(filename, identifier, nodes, location);
        
    }

    public override string ToString()
    {
        return $"link({Filename})";
    }
}
