using System.Diagnostics.CodeAnalysis;
using Reax.Core.Functions;
using Reax.Core.Locations;
using Reax.Core.Modules;

namespace Reax.Core.Ast.Statements;

[ExcludeFromCodeCoverage]
public record ModuleNode(
    string Identifier, 
    Dictionary<string, Function> Functions, 
    SourceLocation Location) : StatementNode(Location), IReaxDeclaration
{
    public override IReaxNode[] Children => [];

    public override void Execute(IReaxExecutionContext context)
    {
        context.SetModule(Identifier, Functions);  
    }

    public override void Serialize(BinaryWriter writer)
    {
        var typename = GetType().AssemblyQualifiedName
            ?? throw new InvalidOperationException("Tipo nulo ao serializar");

        writer.Write(typename);

        writer.Write(Identifier);
        base.Serialize(writer);
    }

    public static new ModuleNode Deserialize(BinaryReader reader)
    {
        var identifier = reader.ReadString();
        var location = ReaxNode.Deserialize(reader);
        return (ModuleNode)ModuleResolver.GetInstance()
            .Resolve(identifier, location);
    }
    
    public void Initialize(IReaxExecutionContext context)
    {
        context.Declare(Identifier);
    }

    public override string ToString()
    {
        return $"import module {Identifier};";
    }
}
