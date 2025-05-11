using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Text;
using Reax.Core.Locations;
using Reax.Parser;
using Reax.Parser.Node;
using Spectre.Console;

namespace Reax.Debugger;

public static class ReaxDebugger
{
    private static Layout _layout;
    private static Table _table;
    private static Action<DebuggerArgs>? _update;
    private static bool _done = false;
    private static bool _toNextLine = false;

    static ReaxDebugger()
    {
        _table = new Table().Centered().Expand().NoBorder();
        _table.AddColumn("Name");
        _table.AddColumn("Immutable");
        _table.AddColumn("Bind");
        _table.AddColumn("Async");
        _table.AddColumn("Category");
        _table.AddColumn("Value");

        var panelTable = new Panel(_table);
        panelTable.Header = new PanelHeader(" [bold blue]Debug[/] - Options: [bold green]Key Down[/] - Next Line | [bold green]Key Right[/] - Next Break | [bold green]Enter[/] - Until the end ");
        panelTable.Expand();

        _layout = new Layout("Root")
            .SplitRows(
                new Layout("Table").Update(panelTable),
                new Layout("Panel")
            );
    }

    public static void Start() 
    {
        AnsiConsole
            .Live(_layout)
            .AutoClear(true)   // Do not remove when done
            .Overflow(VerticalOverflow.Crop) // Show ellipsis when overflowing
            .Cropping(VerticalOverflowCropping.Top) // Crop overflow at top
            .StartAsync(async ctx => 
            {
                _update += (DebuggerArgs args) => Updater(args, ctx);
                while(!_done) 
                {
                    await Task.Delay(1);
                }
                
            }); 
    } 

    private static void Updater(DebuggerArgs args, LiveDisplayContext ctx) 
    {
        var panel = new Panel(PrintStackTrace(args.StackTrace));
        panel.Header = new PanelHeader("[bold blue]Stack Trace[/]");
        panel.Expand = true;                    
        
        _layout["Panel"].Update(panel);
        _table.Rows.Clear();

        foreach (var model in args.Models)
        {
            if(model.Type is not "variable" or "bind" || model.Value == "")
                continue;

            _table.AddRow(
                model.Name, 
                model.Immutable, 
                model.Bind, 
                model.Async, 
                model.Type, 
                model.Value);    
        }

        ctx.Refresh();
    }
    
    public static string PrintStackTrace(IEnumerable<ReaxNode> StackTrace) {
        if(!StackTrace.Any()) return "";
        
        var sb = new StringBuilder();
        foreach (var node in StackTrace) {
            sb.Append("  at ");
            sb.Append(node.Location);
            sb.AppendLine();
        }
        return sb.ToString();
    }

    public static void Debugger(DebuggerArgs args)
    {
        if(!IsBreakPoint(args.Location) && !_toNextLine)
            return;

        _toNextLine = false;
        Console.Clear();
        _update?.Invoke(args);

        ProcessInput();
    }

    private static void ProcessInput() 
    {
        do
        {
            var key = Console.ReadKey();
            if(key.Key == ConsoleKey.RightArrow)
            {
                break;
            }
            else if(key.Key == ConsoleKey.DownArrow)
            {
                _toNextLine = true;
                break;
            }
            else if(key.Key == ConsoleKey.Enter)
            {
                ReaxEnvironment.BreakPoints.Clear();
                break;
            }

        } while(true);
    }
    
    private static bool IsBreakPoint(SourceLocation location) 
    {
        foreach (var file in ReaxEnvironment.BreakPoints.Keys)
        {
            if(location.File.EndsWith(file))
            {
                var lines = ReaxEnvironment.BreakPoints[file];
                if(lines.Contains(location.Start.Line))
                    return true;
            }
        }

        return false;
    }

    public static void Done() 
    {
        _done = true;
    }
}
