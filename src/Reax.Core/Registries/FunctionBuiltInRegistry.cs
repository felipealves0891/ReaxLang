using System.Reflection;
using Reax.Core.Functions;
using Reax.Core.Functions.Attributes;

namespace Reax.Core.Registries;

internal class FunctionBuiltInRegistry : BaseRegistry<string, Dictionary<string, Function>>
{
    protected override Dictionary<string, Function>? Load(string key)
    {
        var functions = this.GetType()
                            .Assembly
                            .GetTypes()
                            .Where(x => x.GetCustomAttribute<FunctionBuiltInAttribute>() is not null
                                     && x.GetCustomAttribute<FunctionBuiltInAttribute>()?.Module == key)
                            .Select(x => 
                            {
                                var attribute = x.GetCustomAttribute<FunctionBuiltInAttribute>() ?? throw new InvalidOperationException();
                                var function = (Activator.CreateInstance(x) as Function) ?? throw new InvalidOperationException();
                                var decorator = new DecorateFunctionBuiltIn(attribute, function);
                                return (attribute.Name, (Function)decorator);
                            }); 
        
        var dictionary = new Dictionary<string, Function>();
        foreach (var item in functions)
            dictionary[item.Name] = item.Item2;

        return dictionary;
    }
}
