using System;
using System.ComponentModel;
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
        
        [Description("Execução com debug ativo.")]
        [CommandOption("-d|--debug")]
        [DefaultValue(false)]
        public bool Debug { get; set; }

        [Description("Caminho para arquivo de configurações.")]
        [CommandOption("-c|--configuration")]
        public string? PathConfigurations { get; set; }
        
        [Description("Os break points devem ser no formato 'arquivo.reax:10,25,32'")]
        [CommandOption("-b|--breakPoints")]
        public string? BreakPoints { get; set; }

    }

    public override ValidationResult Validate(CommandContext context, Settings settings)
    {
        var fileInfo = new FileInfo(settings.ComputedScript);
        if(!fileInfo.Exists)
        {
            return ValidationResult.Error($"Script não encontrado: {settings.ComputedScript}");
        }

        return base.Validate(context, settings);
    }
    
    public override int Execute(CommandContext context, Settings settings)
    {
        var fileInfo = new FileInfo(settings.ComputedScript);

        ReaxEnvironment.DirectoryRoot = fileInfo.DirectoryName ?? throw new Exception();
        ReaxEnvironment.Debug = settings.Debug;
        if(ReaxEnvironment.Debug) 
        {
            Console.WriteLine("--MODO DEBUG ATIVO!\n");
            SetBreakPoints(settings.BreakPoints);
            ReaxDebugger.Start(ReaxEnvironment.BreakPoints);

        }

        var interpreter = ReaxCompiler.Compile(settings.ComputedScript);

        try
        {
            
            interpreter.Interpret();   
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error: "); 
            if(ReaxEnvironment.Debug)
                Console.WriteLine("Reax Error: {0}", ex.Message);

            interpreter.PrintStackTrace();
        }
        finally
        {
            ReaxDebugger.Done();
        }

        return 0;
        
    }

    private void SetBreakPoints(string? bp)
    {
        if(string.IsNullOrEmpty(bp))
            return;

        var options = bp.Split(':');
        if(options.Length != 2)
            return;

        var filename = options[0];
        var lines = options[1].Split(',').Select(x => int.Parse(x)).ToArray();
        ReaxEnvironment.BreakPoints[filename] = new HashSet<int>(lines);
    }

}
