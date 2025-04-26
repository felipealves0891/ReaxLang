using System.Collections.Concurrent;
using Reax.ConsoleDisplay.ConsoleTable;
using Reax.Parser.Node;

namespace Reax.Debugger;

public class ReaxDebugger
{
    public static Table<DebuggerModel> Table { get; private set; } = new();
    public static bool ToNextLine { get; private set; } = false;
    
    static ReaxDebugger() 
    {
        Table.AddItemMenu("Seta para baixo", "Linha a linha");
        Table.AddItemMenu("Enter", "Continuar");
    }

    public static void Debugger(DebuggerArgs args)
    {
        var source = args.Location;
        if(!ToNextLine)
        {
            if(!ReaxEnvironment.BreakPoints.TryGetValue(source.File, out var lines) || !lines.Contains(source.Line))
            {
                return;
            }
        }
    
        Console.Clear();
        Table.SetData(args.Models);
        Table.Print();
        Console.WriteLine("|");
        Console.WriteLine();
        Console.WriteLine(source);
        PrintStackTrace(args.StackTrace);

        ToNextLine = false;
        if(Console.ReadKey().Key == ConsoleKey.DownArrow)
            ToNextLine = true;
    }

    public static void PrintStackTrace(ConcurrentStack<ReaxNode> StackTrace) {
        if(!StackTrace.Any()) return;
        foreach (var node in StackTrace.Reverse()) {
            Console.WriteLine($"  at {node.Location.File}:{node.Location.Line}:{node.Location.Position} -> {node.ToString()}");
        }
    }
}
