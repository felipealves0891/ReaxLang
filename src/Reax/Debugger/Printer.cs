using System;
using System.Reflection;

namespace Reax.Debugger;

public static class Printer
{
    public static void PrintTable<TData>(this IEnumerable<TData> values, IDictionary<string, int>? propLength = null)
    {
        propLength ??= values.MaxLengthForColumn();
        var columns = typeof(TData).GetPropertyNames();

        Console.WriteLine();
        foreach (var item in values)
        {
            foreach (var column in columns)
            {
                var columnLength = propLength[column];
                typeof(TData).GetProperty(column)?.Print(columnLength, item);
                
            }

            Console.WriteLine();
        }
    }

    public static IDictionary<string, int> PrintTableHeader<TData>(this IEnumerable<TData> list)
    {
        var propLength = list.MaxLengthForColumn();
        var columns = typeof(TData).GetPropertyNames();

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

    private static IDictionary<string, int> MaxLengthForColumn<TData>(this IEnumerable<TData> values)
    { 
        var list = typeof(TData).GetPropertyNames();

        var props = new Dictionary<string, int>();
        foreach (var item in list)
            props[item] = 5;

        foreach (var value in values)
        {
            foreach (var propKey in props.Keys)
            {
                var propValue = value?.GetType()?.GetProperty(propKey)?.GetValue(value)?.ToString() ?? "";
                if(propValue.Length > props[propKey])
                    props[propKey] = propValue.Length + 3;

                if(propKey.ToString().Length >= props[propKey])
                    props[propKey] = propKey.ToString().Length + 3;
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
    
    private static IList<string> GetPropertyNames(this Type type) 
    {
        return type
                .GetProperties()
                .Select(x => x.Name)
                .ToList();
    }

    
}
