using System;
using System.Reflection;

namespace Reax.ConsoleDisplay.ConsoleTable;

public class Table<TData>
    where TData : class
{
    private readonly IDictionary<PropertyInfo, int> _columns = new Dictionary<PropertyInfo, int>();
    private IList<TData> Data = new List<TData>();

    public Table()
    {
        var columns = typeof(TData).GetProperties();       
        foreach (var column in columns)
            _columns.Add(column, column.Name.Length + 3);
    }

    public void SetData(IEnumerable<TData> data)
    {
        foreach (var item in data)
        {
            foreach (var key in _columns.Keys)
            {
                var contentLength = key.GetValue(item)?.ToString()?.Length ?? 0;
                if(_columns[key] < contentLength)
                    _columns[key] = contentLength + 3;
            }
        }

        Data = new List<TData>(data);
    }

    public void Print()
    {
        DrawHeader();
        DrawData();
    }

    private void DrawHeader()
    {
        foreach (var key in _columns.Keys)
            DrawColumn("", _columns[key], '-');

        Console.WriteLine("|");
        foreach (var key in _columns.Keys)
            DrawColumn(key.Name, _columns[key]);
        
        Console.WriteLine("|");
        foreach (var key in _columns.Keys)
            DrawColumn("", _columns[key], '-');

        Console.WriteLine("|");

    }

    private void DrawData() 
    {
        foreach (var item in Data)
        {
            foreach (var key in _columns.Keys)
            {
                DrawContent(key, _columns[key], item);
            }

            Console.WriteLine("|");
        }
        
        foreach (var key in _columns.Keys)
            DrawColumn("", _columns[key], '-');

    }

    private void DrawContent(PropertyInfo property, int length, object? item) 
    {
        if(property is null || item is null)
        {
            DrawColumn("UNDEFINED", length);
            return;
        }

        var value = property.GetValue(item);
        if(value is null)
        {
            DrawColumn("", length);
            return;
        }

        var text = value.ToString();
        if(text is null)
        {
            DrawColumn("", length);
            return;
        }

        DrawColumn(text, length);
    }
    
    private static void DrawColumn(string value, int length, char fillment = ' ', bool includeSeparetor = true)
    {
        if(includeSeparetor) 
        {
            Console.Write("|");
            length--;
        }

        if(value.Length <= length)
            Console.Write(value.PadRight(length, fillment));
        else
            Console.Write(value.Substring(0, length));
    }


}
