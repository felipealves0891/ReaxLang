using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Text;
using Reax.Parser;
using Reax.Parser.Node;
using Spectre.Console;

namespace Reax.Debugger;

public static class ReaxDebugger
{
    private static Action<DebuggerArgs>? _update;
    private static bool _done = false;
    private static Task? _console;

    public static void Start() 
    {
        var table = new Table().Centered().Expand().NoBorder();
        table.AddColumn("Name");
        table.AddColumn("Immutable");
        table.AddColumn("Bind");
        table.AddColumn("Async");
        table.AddColumn("Category");
        table.AddColumn("Value");

        var panelTable = new Panel(table);
        panelTable.Header = new PanelHeader("[bold blue]Debug[/]");
        panelTable.Expand();

        var layout = new Layout("Root")
            .SplitRows(
                new Layout("Table").Update(panelTable),
                new Layout("Panel")
            );

        _console = AnsiConsole
            .Live(layout)
            .AutoClear(true)   // Do not remove when done
            .Overflow(VerticalOverflow.Crop) // Show ellipsis when overflowing
            .Cropping(VerticalOverflowCropping.Top) // Crop overflow at top
            .StartAsync(async ctx => 
            {
                _update += (DebuggerArgs args) => 
                {
                    var panel = new Panel(PrintStackTrace(args.StackTrace));
                    panel.Header = new PanelHeader("[bold blue]Stack Trace[/]");
                    panel.Expand = true;                    
                    layout["Panel"].Update(panel);

                    table.Rows.Clear();
                    foreach (var model in args.Models)
                    {
                        if(model.Type is not "variable" or "bind" || model.Value == "")
                            continue;

                        table.AddRow(
                            model.Name, 
                            model.Immutable, 
                            model.Bind, 
                            model.Async, 
                            model.Type, 
                            model.Value);    
                    }

                    ctx.Refresh();
                };

                while(!_done) 
                {
                    await Task.Delay(1);
                }
                
            }); 
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
        Console.Clear();
        _update?.Invoke(args);
        Task.Delay(500).Wait();
    }

    public static void Done() 
    {
        _done = true;
        _console?.Wait();
    }
}
