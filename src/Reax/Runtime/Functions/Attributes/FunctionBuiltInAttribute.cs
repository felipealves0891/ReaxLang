using System;

namespace Reax.Runtime.Functions.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class FunctionBuiltInAttribute : Attribute
{
    public FunctionBuiltInAttribute(string name, bool canReturn)
    {
        Name = name;
        CanReturn = canReturn;
        MinParametersCount = 0;
        MaxParametersCount = 0;
    }

    public FunctionBuiltInAttribute(string name, bool canReturn, int parametersCount)
    {
        Name = name;
        CanReturn = canReturn;
        MinParametersCount = parametersCount;
        MaxParametersCount = parametersCount;
    }

    public FunctionBuiltInAttribute(string name, bool canReturn, int minParametersCount = 0, int maxParametersCount = 0)
    {
        Name = name;
        CanReturn = canReturn;
        MinParametersCount = minParametersCount;
        MaxParametersCount = maxParametersCount;
    }

    public string Name { get; private set; }

    public bool CanReturn { get; private set; }

    public int? MinParametersCount { get; set; }
    
    public int? MaxParametersCount { get; set; }
}
