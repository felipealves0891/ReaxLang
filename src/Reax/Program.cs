using Reax.Commands;
using Spectre.Console.Cli;

var app = new CommandApp<RunCommand>();
return app.Run(args);

// dotnet run -- "D:\Source\Scripts\simple.reax" -b