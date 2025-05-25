using System.Reflection;
using Reax.Core.Ast;

namespace Reax.Core.Helpers;

public static class BinaryDeserializerHelper
{
    public static T Deserialize<T>(BinaryReader reader) where T : ReaxNode
    {
        var typeName = reader.ReadString();
        var type = Type.GetType(typeName)
            ?? throw new InvalidOperationException($"Tipo '{typeName}' não encontrado.");

        if (type.GetMethod("Deserialize", BindingFlags.Public | BindingFlags.Static) is not { } deserializeMethod)
            throw new InvalidOperationException($"O tipo '{typeName}' não possui um método estático 'Deserialize'.");

        if (deserializeMethod.ReturnType != typeof(T))
            throw new InvalidOperationException($"O método 'Deserialize' do tipo '{typeName}' não retorna o tipo esperado '{typeof(T)}'.");
        
        var instance = deserializeMethod.Invoke(null, new object[] { reader })
            ?? throw new InvalidOperationException($"O método 'Deserialize' do tipo '{typeName}' retornou nulo.");

        return instance is T result
            ? result
            : throw new InvalidCastException($"O tipo '{typeName}' não é do tipo esperado '{typeof(T)}'.");
    }
}
