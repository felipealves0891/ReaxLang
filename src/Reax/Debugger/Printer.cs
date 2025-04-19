using System;
using System.Reflection;

namespace Reax.Debugger;

public static class Printer
{
    public static void PrintTable<TKey, TData>(this IReadOnlyDictionary<TKey, TData> dic, IDictionary<string, int>? propLength = null) 
        where TKey : notnull
    {
        if(propLength is null) 
        {
            propLength ??= dic.MaxLengthForColumn();
            propLength["_Key"] = dic.Keys.Select(x => x.ToString()).Select(x => x is null ? 0 : x.Length + 1).Max(); 
        }
        
        var first = dic.First().Value;
        var columns = typeof(TData)
                        .GetProperties()
                        .Select(x => x.Name)
                        .Reverse()
                        .ToList();

        columns.Add("_Key");
        columns.Reverse();

        Console.WriteLine();
        foreach (var key in dic.Keys)
        {
            var item = dic[key];
            foreach (var column in columns)
            {
                var columnLength = propLength[column];
                if(column == "_Key")
                {
                    key.ToString()?.PrintColumn(columnLength);
                    continue;
                }

                typeof(TData).GetProperty(column)?.Print(columnLength, item);
                
            }

            Console.WriteLine();
        }
    }

    public static IDictionary<string, int> PrintTableHeader<TKey, TData>(this IReadOnlyDictionary<TKey, TData> dic)
        where TKey : notnull
    {
        var propLength = dic.MaxLengthForColumn();
        var columns = typeof(TData).GetPropertyNames().Reverse().ToList();

        propLength["_Key"]  = dic.Keys.Select(x => x.ToString()).Select(x => x is null ? 0 : x.Length +1).Max(); 
        columns.Add("_Key");
        columns.Reverse();

        for (int i = 0; i < columns.Count; i++)
            "|".PrintColumn(propLength[columns[i]], '-', false);

        Console.WriteLine();
        foreach (var column in columns)
            column.PrintColumn(propLength[column], ' ');
        
        Console.WriteLine();
        for (int i = 0; i < columns.Count; i++)
            "|".PrintColumn(propLength[columns[i]], '-', false);

        return propLength;
        
    }

    private static IList<string> GetPropertyNames(this Type type) 
    {
        return type
                .GetProperties()
                .Select(x => x.Name)
                .ToList();
    }

    private static IDictionary<string, int> MaxLengthForColumn<TKey, TData>(this IReadOnlyDictionary<TKey, TData> dic)
        where TKey : notnull
    { 
        var list = typeof(TData)
                        .GetProperties()
                        .Select(x => x.Name)
                        .ToList();

        var props = new Dictionary<string, int>();
        foreach (var item in list)
            props[item] = 5;

        foreach (var key in dic.Keys)
        {
            var value = dic[key];
            foreach (var propKey in props.Keys)
            {
                int length = props[propKey];
                var propValue = value?.GetType()?.GetProperty(propKey)?.GetValue(value)?.ToString() ?? "";
                if(propValue.Length > props[propKey])
                    props[propKey] = propValue.Length + 2;

                if(propKey.ToString().Length >= props[propKey])
                    props[propKey] = propKey.ToString().Length + 2;
            }            
        }

        return props;
    }

    private static void Print(this PropertyInfo property, int length, object? item)
    {
        if(property is null || item is null)
        {
            "UNDEFINED".PrintColumn(length);
            return;
        }

        var value = property.GetValue(item);
        if(value is null)
        {
            "".PrintColumn(length);
            return;
        }

        var text = value.ToString();
        if(text is null)
        {
            "".PrintColumn(length);
            return;
        }

        text.PrintColumn(length);
    }

    private static void PrintColumn(this string value, int length, char fillment = ' ', bool includeSeparetor = true)
    {
        if(includeSeparetor) 
        {
            Console.Write("|");
            length--;
        }
        Console.Write(value.PadRight(length, fillment));
    }

    
}
