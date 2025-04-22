using System;
using System.Runtime.CompilerServices;

namespace Reax.Debugger;

public enum LoggerLevel 
{
    NONE = -2,
    DEBUG,
    INFO,
    ERROR
}

public static class Logger
{
    public static bool Enabled = true;
    public static string FormatDate = "HH:mm:ss.ffffff";
    public static LoggerLevel Level = LoggerLevel.DEBUG;

    public static void LogLexer(string message, [CallerMemberName] string caller = "") 
    {
        if(!Enabled) return;
        var formateDate = DateTime.UtcNow.ToString(FormatDate);
        var done = string.Format("DEB [{0}] | Lexer.{2} | {1}", formateDate, message, caller.PadRight(25, ' '));
        Log(done, LoggerLevel.DEBUG);
    }
    
    public static void LogParse(string message, [CallerMemberName] string caller = "") 
    {
        if(!Enabled) return;
        var formateDate = DateTime.UtcNow.ToString(FormatDate);
        var done = string.Format("DEB [{0}] | Parse.{2} | {1}", formateDate, message, caller.PadRight(25, ' '));
        Log(done, LoggerLevel.DEBUG);
    }

    public static void LogAnalize(string message, [CallerMemberName] string caller = "") 
    {
        if(!Enabled) return;
        var formateDate = DateTime.UtcNow.ToString(FormatDate);
        var done = string.Format("DEB [{0}] | Analizer.{2} | {1}", formateDate, message, caller.PadRight(22, ' '));
        Log(done, LoggerLevel.DEBUG);
    }
    
    public static void LogCompile(string message, [CallerMemberName] string caller = "") 
    {
        if(!Enabled) return;
        var formateDate = DateTime.UtcNow.ToString(FormatDate);
        var done = string.Format("INF [{0}] | Compile.{2} | {1}", formateDate, message, caller.PadRight(23, ' '));
        Log(done, LoggerLevel.INFO);
    }

    public static void Log(string message, LoggerLevel level) 
    {
        if(((int)level) >= ((int)Level))
        {
            if(level == LoggerLevel.DEBUG) Console.ForegroundColor = ConsoleColor.Blue;
            if(level == LoggerLevel.INFO) Console.ForegroundColor = ConsoleColor.Green;
            if(level == LoggerLevel.ERROR) Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();
        }
            
    }
    
}
