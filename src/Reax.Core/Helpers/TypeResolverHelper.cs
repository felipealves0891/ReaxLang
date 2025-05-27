using System;
using System.Reflection;
using System.Linq;
using Reax.Core.Types;
using System.Collections.Immutable;
using Reax.Core.Ast;
using Reax.Core.Ast.Interfaces;
using Reax.Core.Ast.Literals;
using Reax.Core.Locations;
using Reax.Core.Ast.Objects;
using Reax.Parser.Node;

public static class TypeResolverHelper
{
    public static Type? GetTypeFromName(string typeName)
    {
        if (string.IsNullOrWhiteSpace(typeName))
            return null;

        typeName = typeName.Trim().ToLowerInvariant();
        var type = typeName switch
        {
            "int" or "int32" => typeof(int),
            "long" or "int64" => typeof(long),
            "short" or "int16" => typeof(short),
            "byte" => typeof(byte),
            "bool" or "boolean" => typeof(bool),
            "string" => typeof(string),
            "double" => typeof(double),
            "float" or "single" => typeof(float),
            "decimal" => typeof(decimal),
            "char" => typeof(char),
            "datetime" => typeof(DateTime),
            "object" => typeof(object),
            _ => Type.GetType(typeName, throwOnError: false, ignoreCase: true)
        };

        if (type is not null)
            return type;

        return AppDomain.CurrentDomain
                     .GetAssemblies()
                     .SelectMany(a => a.GetTypes())
                     .FirstOrDefault(t => t.Name.Equals(typeName, StringComparison.InvariantCultureIgnoreCase));
    }

    public static MemberInfo? GetMemberInfo(Type type, string memberName, Type[]? parameterTypes)
    {
        if (type == null || string.IsNullOrWhiteSpace(memberName))
            return null;

        var bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

        // Se tipos de parâmetros forem fornecidos, buscamos por método correspondente (sobrecarga)
        if (parameterTypes != null && parameterTypes.Any())
        {
            var member = type
                .GetMethods(bindingFlags)
                .FirstOrDefault(m =>
                    string.Equals(m.Name, memberName, StringComparison.OrdinalIgnoreCase) &&
                    m.GetParameters().Select(p => p.ParameterType).SequenceEqual(parameterTypes));

            if (member != null)
                return member;
        }

        return GetMemberInfo(type, memberName);
    }

    public static MemberInfo? GetMemberInfo(Type type, string memberName)
    {
        if (type == null || string.IsNullOrWhiteSpace(memberName))
            return null;

        return type
            .GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
            .FirstOrDefault(m => string.Equals(m.Name, memberName, StringComparison.OrdinalIgnoreCase));
    }

    public static DataType GetReaxTypeByMember(MemberInfo member)
    {
        Type? memberType = member switch
        {
            PropertyInfo prop => prop.PropertyType,
            FieldInfo field => field.FieldType,
            MethodInfo method => method is MethodInfo mi ? mi.ReturnType : null,
            _ => null
        };

        if (memberType == null)
            return DataType.NONE;

        return GetReaxTypeByType(memberType);
    }

    public static DataType GetReaxTypeByType(Type memberType)
    {
        if (memberType == typeof(void))
            return DataType.VOID;

        if (IsNumericType(memberType))
            return DataType.NUMBER;

        if (memberType == typeof(string) || memberType == typeof(char))
            return DataType.STRING;

        if (memberType == typeof(bool))
            return DataType.BOOLEAN;

        if (typeof(IEnumerable<>).IsAssignableFrom(memberType))
            return DataType.ARRAY;

        return DataType.NONE;
    }

    private static bool IsNumericType(Type type)
    {
        return type == typeof(byte) || type == typeof(sbyte) ||
               type == typeof(short) || type == typeof(ushort) ||
               type == typeof(int) || type == typeof(uint) ||
               type == typeof(long) || type == typeof(ulong) ||
               type == typeof(float) || type == typeof(double) ||
               type == typeof(decimal);
    }

    public static object? InvokeMember(object? target, MemberInfo member, params object[] parameters)
    {
        return member switch
        {
            PropertyInfo prop => prop.GetValue(target),
            FieldInfo field => field.GetValue(target),
            MethodInfo method => InvokeWithParsedParameters(method, target, parameters),
            _ => null
        };
    }

    public static object? InvokeWithParsedParameters(MethodInfo method, object? instance, object[] parameters)
    {
        if (method == null)
            throw new ArgumentNullException(nameof(method));

        var paramInfos = method.GetParameters();
        if (paramInfos.Length != parameters.Length)
            throw new ArgumentException("Parameter count mismatch.");

        object?[] parsedParams = new object?[parameters.Length];
        for (int i = 0; i < parameters.Length; i++)
        {
            var targetType = paramInfos[i].ParameterType;
            var value = parameters[i];

            if (value == null || targetType.IsInstanceOfType(value))
            {
                parsedParams[i] = value;
            }
            else
            {
                // Se o valor for um array e o tipo de destino também for array, converta cada item individualmente
                if (value is Array valueArray && targetType.IsArray)
                {
                    var elementType = targetType.GetElementType();
                    var convertedArray = Array.CreateInstance(elementType!, valueArray.Length);
                    for (int j = 0; j < valueArray.Length; j++)
                    {
                        convertedArray.SetValue(Convert.ChangeType(valueArray.GetValue(j), elementType!), j);
                    }
                    parsedParams[i] = convertedArray;
                }
                else
                {
                    parsedParams[i] = Convert.ChangeType(value, targetType);
                }
                

            }
        }

        return method.Invoke(instance, parsedParams);
    }

    public static IReaxValue CastToReax(object result, string resultText, DataType reaxType, SourceLocation location)
    {
        return reaxType switch
        {
            DataType.STRING => new StringNode(resultText, location),
            DataType.NUMBER => new NumberNode(resultText, location),
            DataType.BOOLEAN => new BooleanNode(resultText, location),
            _ => reaxType.HasFlag(DataType.ARRAY)
               ? new ArrayNode(GetLiterals(result, location), location)
               : new NullNode(location),
        };
    }
    
    private static ImmutableArray<ReaxNode> GetLiterals(object result, SourceLocation location)
    {
        var enumerable = (IEnumerable<object>)result;
        if (!enumerable.Any())
            return new List<ReaxNode>().ToImmutableArray();

        var nativeType = enumerable.First().GetType();
        var reaxType = TypeResolverHelper.GetReaxTypeByType(nativeType);

        return enumerable
            .Select(x => CastToReax(x, x.ToString() ?? "", reaxType, location))
            .Cast<ReaxNode>()
            .ToImmutableArray();
    }

}
