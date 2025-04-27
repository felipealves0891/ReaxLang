using System;

namespace Reax.Runtime.Functions.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class FunctionBuiltInAttribute : Attribute
{
    public FunctionBuiltInAttribute(
        string module, 
        string name)
    {
        Name = name;
        Module = module;
        MinParametersCount = 0;
        MaxParametersCount = 0;
    }

    public FunctionBuiltInAttribute(
        string module, 
        string name, 
        int parametersCount)
    {
        Module = module;
        Name = name;
        MinParametersCount = parametersCount;
        MaxParametersCount = parametersCount;
    }

    public FunctionBuiltInAttribute(
        string module, 
        string name, 
        int minParametersCount, 
        int maxParametersCount)
    {
        Module = module;
        Name = name;
        MinParametersCount = minParametersCount;
        MaxParametersCount = maxParametersCount;
    }


    public string Module { get; private set; }
    public string Name { get; private set; }
    public int MinParametersCount { get; private set; }
    public int MaxParametersCount { get; private set; }
}
