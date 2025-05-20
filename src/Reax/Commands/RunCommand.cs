using System;
using System.ComponentModel;
using System.Diagnostics;
using Reax.Core;
using Reax.Core.Debugger;
using Reax.Interpreter;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Reax.Commands;

public sealed class RunCommand : Command<RunCommand.Settings>
{
    public sealed class Settings : CommandSettings
    {
        public string ComputedScript => Script ?? Path.Combine(Directory.GetCurrentDirectory(), "main.reax");

        [Description("Caso não seja passado o script a ser executado, sera assumido que o main.reax sera executado.")]
        [CommandArgument(0, "[script]")]
        public string? Script { get; set; }

        [Description("Define o nivel de log. Trace=-2, Debug=-1, Info=0, Error=1, None=2")]
        [CommandOption("-l|--loglevel")]
        [DefaultValue(1)]
        public int LogLevel { get; set; }
    }

    public override ValidationResult Validate(CommandContext context, Settings settings)
    {
        var fileInfo = new FileInfo(settings.ComputedScript);
        if(!fileInfo.Exists)
        {
            return ValidationResult.Error($"Script não encontrado: {settings.ComputedScript}");
        }

        if (settings.LogLevel < -2 || settings.LogLevel > 2)
        { 
            return ValidationResult.Error($"Log Level invalido: Trace=-2, Debug=-1, Info=0, Error=1, None=2");
        }

        return base.Validate(context, settings);
    }
    
    public override int Execute(CommandContext context, Settings settings)
    {
        TimeSpan buildTime = TimeSpan.Zero;
        TimeSpan runTime = TimeSpan.Zero;
        var fileInfo = new FileInfo(settings.ComputedScript);

        ReaxEnvironment.DirectoryRoot = fileInfo.DirectoryName ?? throw new Exception();
        Logger.Level = (LoggerLevel)settings.LogLevel;

        IReaxInterpreter interpreter = null!;
        Stopwatch stopwatch = new Stopwatch();

        try
        {
            stopwatch.Start();
            interpreter = ReaxCompiler.Compile(settings.ComputedScript);
            buildTime = stopwatch.Elapsed;
            interpreter.Interpret();
            runTime = stopwatch.Elapsed - buildTime;
            stopwatch.Stop();
        }
        catch (Exception ex)
        {
            if (stopwatch.IsRunning)
                stopwatch.Stop();

            Logger.LogError(ex, "Error: ");
            Console.WriteLine("Reax Error: {0}", ex.Message);

            if (interpreter is not null)
                Console.WriteLine(interpreter.PrintStackTrace());
        }

        Console.WriteLine("\nBuild Time: {0}", buildTime);
        Console.WriteLine("Run Time: {0}", runTime);
        Console.WriteLine("Total Time: {0}", stopwatch.Elapsed);

        return 0;
        
    }
}
