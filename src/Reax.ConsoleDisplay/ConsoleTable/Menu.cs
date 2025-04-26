using System;
using System.Text;

namespace Reax.ConsoleDisplay.ConsoleTable;

public class ItemMenu 
{
    public ItemMenu(string name, string action)
    {
        Name = name;
        Action = action;
    }

    public string Name { get; init; }
    public string Action { get; init; }
}

public class Menu
{
    private readonly List<ItemMenu> _items;

    public Menu()
    {
        _items = new List<ItemMenu>();
    }

    public void SetAction(string name, string action)
    {
        _items.Add(new ItemMenu(name, action));
    }

    public void Print(int width) 
    {
        Console.WriteLine("|".PadRight(width, '-'));
        
        StringBuilder sb = new StringBuilder();
        sb.Append("|");

        foreach (var item in _items)
        {
            sb.Append(string.Format("   -{0}: {1}", item.Name, item.Action));
        }

        Console.Write(sb.ToString().PadRight(width, ' '));
        Console.WriteLine("|");
    }

    
}
