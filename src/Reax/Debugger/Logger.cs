using System;
using System.Runtime.CompilerServices;

namespace Reax.Debugger;

public enum LoggerLevel 
{
    DEBUG = -2,
    INFO,
    ERROR,
    NONE
}

public sealed class Logger : IDisposable
{
    private static Logger _instance = new Logger();
    public static bool Enabled = true;
    public static string FormatDate = "yyyy-MM-dd HH:mm:ss.ffffff";
    public static LoggerLevel Level = LoggerLevel.DEBUG;

    public static void LogLexer(string message, [CallerMemberName] string caller = "") 
    {
        if(!Enabled) return;
        var formateDate = DateTime.UtcNow.ToString(FormatDate);
        var done = string.Format("DEB [{0}] | Lexer.{2} | {1}", formateDate, message, caller.PadRight(25, ' '));
        _instance.Log(done, LoggerLevel.DEBUG);
    }
    
    public static void LogParse(string message, [CallerMemberName] string caller = "") 
    {
        if(!Enabled) return;
        var formateDate = DateTime.UtcNow.ToString(FormatDate);
        var done = string.Format("DEB [{0}] | Parse.{2} | {1}", formateDate, message, caller.PadRight(25, ' '));
        _instance.Log(done, LoggerLevel.DEBUG);
    }

    public static void LogAnalize(string message, [CallerMemberName] string caller = "") 
    {
        if(!Enabled) return;
        var formateDate = DateTime.UtcNow.ToString(FormatDate);
        var done = string.Format("DEB [{0}] | Analizer.{2} | {1}", formateDate, message, caller.PadRight(22, ' '));
        _instance.Log(done, LoggerLevel.DEBUG);
    }
    
    public static void LogInterpreter(string message, [CallerMemberName] string caller = "") 
    {
        if(!Enabled) return;
        var formateDate = DateTime.UtcNow.ToString(FormatDate);
        var done = string.Format("INF [{0}] | Interpreter.{2} | {1}", formateDate, message, caller.PadRight(20, ' '));
        _instance.Log(done, LoggerLevel.INFO);
    }
    
    public static void LogCompile(string message, [CallerMemberName] string caller = "") 
    {
        if(!Enabled) return;
        var formateDate = DateTime.UtcNow.ToString(FormatDate);
        var done = string.Format("INF [{0}] | Compile.{2} | {1}", formateDate, message, caller.PadRight(23, ' '));
        _instance.Log(done, LoggerLevel.INFO);
    }

    public static void LogError(Exception exception, string message, [CallerMemberName] string caller = "")
    {
        var formateDate = DateTime.UtcNow.ToString(FormatDate);
        var done = string.Format("ERR [{0}] | Compile.{2} | {1}\n{3}", formateDate, message, caller.PadRight(23, ' '), exception);
        _instance.Log(done, LoggerLevel.ERROR);
    }

    private readonly StreamWriter _writer = new StreamWriter(@"D:\Source\Scripts\reax-execution.log", true);
    private void Log(string message, LoggerLevel level) 
    {
        if(((int)level) >= ((int)Level))
        {
            _writer.WriteLine(message);
            _writer.Flush();
        }            
    }

    public void Dispose()
    {
        _writer.Dispose();
    }
}
