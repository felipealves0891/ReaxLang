using System;
using Reax.Semantic.Symbols;

namespace Reax.Runtime.Functions.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class FunctionBuiltInAttribute : Attribute
{
    public FunctionBuiltInAttribute(
        string module, 
        string name, 
        SymbolType returnType, 
        params SymbolType[] parameterTypes)
    {
        Name = name;
        ReturnType = returnType;
        ParameterTypes = parameterTypes;
        Module = module;
        MinParametersCount = 0;
        MaxParametersCount = 0;
    }

    public FunctionBuiltInAttribute(
        string module, 
        string name, 
        int parametersCount, 
        SymbolType returnType, 
        params SymbolType[] parameterTypes)
    {
        Module = module;
        Name = name;
        ReturnType = returnType;
        ParameterTypes = parameterTypes;
        MinParametersCount = parametersCount;
        MaxParametersCount = parametersCount;
    }

    public FunctionBuiltInAttribute(
        string module, 
        string name, 
        int minParametersCount, 
        int maxParametersCount, 
        SymbolType returnType, 
        params SymbolType[] parameterTypes)
    {
        Module = module;
        Name = name;
        ReturnType = returnType;
        ParameterTypes = parameterTypes;
        MinParametersCount = minParametersCount;
        MaxParametersCount = maxParametersCount;
    }


    public string Module { get; private set; }
    public string Name { get; private set; }
    public SymbolType ReturnType { get; }
    public SymbolType[] ParameterTypes { get; }
    public int MinParametersCount { get; private set; }
    public int MaxParametersCount { get; private set; }
}
