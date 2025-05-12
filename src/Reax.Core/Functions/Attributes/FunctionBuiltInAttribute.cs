using System;
using Reax.Core.Types;
using Reax.Parser;

namespace Reax.Core.Functions.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class FunctionBuiltInAttribute : Attribute
{
    public Type? TypeSource { get; set; }
    public string? ResultProperty { get; set; }
    public string? ParametersProperty { get; set; }

    public FunctionBuiltInAttribute(
        string module, 
        string name)  : this(module, name, 0, 0)
    {
    }

    public FunctionBuiltInAttribute(
        string module, 
        string name, 
        int parametersCount) : this(module, name, parametersCount, parametersCount)
    {
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
    

    public DataType? GetResult() 
    {
        if(TypeSource is null || ResultProperty is null)
            return null;

        var property = TypeSource.GetProperty(ResultProperty);
        if(property is null)
            return null;

        var getter = property.GetGetMethod();
        if(getter is null || !getter.IsStatic)
            return null;

        var value = property.GetValue(null);
        if(value is DataType dataType)
            return dataType;
        else
            return null;
    }
    public DataType[] GetParameters() 
    {
        if(TypeSource is null || ParametersProperty is null)
            return [];

        var property = TypeSource.GetProperty(ParametersProperty);
        if(property is null)
            return [];

        var getter = property.GetGetMethod();
        if(getter is null || !getter.IsStatic)
            return [];

        var value = property.GetValue(null);
        if(value is DataType[] dataType)
            return dataType;
        else
            return [];
    }
}
