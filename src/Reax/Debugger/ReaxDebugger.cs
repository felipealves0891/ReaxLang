using System.Collections.Concurrent;
using Reax.ConsoleDisplay.ConsoleTable;
using Reax.Parser.Node;

namespace Reax.Debugger;

public class ReaxDebugger
{
    public static Table<DebuggerModel> Table { get; private set; } = new();
    public static bool ToNextLine { get; private set; } = false;
    public static ConsoleKey[] Actions = [ConsoleKey.DownArrow, ConsoleKey.Enter, ConsoleKey.LeftArrow];

    static ReaxDebugger() 
    {
        Table.AddItemMenu(ConsoleKey.DownArrow.ToString(), "Linha a linha");
        Table.AddItemMenu(ConsoleKey.RightArrow.ToString(), "Continuar");
        Table.AddItemMenu(ConsoleKey.Enter.ToString(), "Continuar até o fim!");
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
        Execute();
    }

    private static void Execute() 
    {
        ToNextLine = false;
        
        ConsoleKey key;
        do
        {
            key = Console.ReadKey().Key;
            if(key == ConsoleKey.DownArrow)
            {
                ToNextLine = true;
                break;
            }
            else if(key == ConsoleKey.RightArrow)
            {
                break;
            }
            else if(key == ConsoleKey.Enter)
            {
                ReaxEnvironment.BreakPoints.Clear();
                break;
            }
        }
        while(!Actions.Contains(key));

    }

    public static void PrintStackTrace(ConcurrentStack<ReaxNode> StackTrace) {
        if(!StackTrace.Any()) return;
        foreach (var node in StackTrace.Reverse()) {
            Console.WriteLine($"  at {node.Location.File}:{node.Location.Line}:{node.Location.Position} -> {node.ToString()}");
        }
    }
}
