using Reax.Commands;
using Reax.Core.Debugger;
using Spectre.Console.Cli;

using (Logger.Instance)
{
    var app = new CommandApp<RunCommand>();
    return app.Run(args);
}
// dotnet run -- "D:\Source\Scripts\simple.reax" -b