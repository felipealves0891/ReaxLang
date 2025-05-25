using System.Diagnostics.CodeAnalysis;
using Reax.Core.Functions;
using Reax.Core.Locations;
using Reax.Core.Registries;


namespace Reax.Core.Ast.Statements;

[ExcludeFromCodeCoverage]
public record ModuleNode(
    string identifier, 
    Dictionary<string, Function> functions, 
    SourceLocation Location) : StatementNode(Location), IReaxDeclaration
{
    private static BuiltInRegistry _builtInRegistry = new ();

    public override IReaxNode[] Children => [];

    public override void Execute(IReaxExecutionContext context)
    {
        context.Declare(identifier);
        context.SetModule(identifier, functions);  
    }

    public override void Serialize(BinaryWriter writer)
    {
        var typename = GetType().AssemblyQualifiedName
            ?? throw new InvalidOperationException("Tipo nulo ao serializar");

        writer.Write(typename);

        writer.Write(identifier);
        base.Serialize(writer);
    }

    public static new ModuleNode Deserialize(BinaryReader reader)
    {
        var identifier = reader.ReadString();
        var functions = _builtInRegistry.Get(identifier);
        var location = ReaxNode.Deserialize(reader);
        return new ModuleNode(identifier, functions, location);
    }

    public override string ToString()
    {
        return $"import module {identifier};";
    }
}
