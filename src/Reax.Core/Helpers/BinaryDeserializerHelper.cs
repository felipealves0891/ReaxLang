using System.Reflection;
using Reax.Core.Ast;
using Reax.Core.Debugger;

namespace Reax.Core.Helpers;

public static class BinaryDeserializerHelper
{
    public static T Deserialize<T>(BinaryReader reader) where T : ReaxNode
    {
        var typeName = reader.ReadString();
        Logger.LogAnalize($"Deserializando tipo: {typeName}");

        var type = Type.GetType(typeName)
            ?? throw new InvalidOperationException($"Tipo '{typeName}' não encontrado.");

        if (type.GetMethod("Deserialize", BindingFlags.Public | BindingFlags.Static) is not { } deserializeMethod)
            throw new InvalidOperationException($"O tipo '{typeName}' não possui um método estático 'Deserialize'.");

        var instance = deserializeMethod.Invoke(null, new object[] { reader })
            ?? throw new InvalidOperationException($"O método 'Deserialize' do tipo '{typeName}' retornou nulo.");

        Logger.LogAnalize($"Deserializado tipo: {instance}");
        return instance is T result
            ? result
            : throw new InvalidCastException($"O tipo '{typeName}' não é do tipo esperado '{typeof(T)}'.");
    }
}
