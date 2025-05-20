using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace Reax.Core.Debugger;

public enum LoggerLevel 
{
    TRACE = -2,
    DEBUG,
    INFO,
    ERROR,
    NONE
}

public sealed class Logger : IDisposable
{
    private readonly StreamWriter _writer;
    private readonly Timer _timer;
    private readonly object _locker = new();

    public Logger()
    {
        try
        {
            var path = ReaxEnvironment.DirectoryRoot;
            var filename = new FileInfo(Path.Combine(path, "logs", $"{DateTime.Now.ToString("yyyy-MM-dd-HHmmss")}.log"));
            if (filename.Directory is not null && !filename.Directory.Exists)
                filename.Directory.Create();
            
            _writer = new StreamWriter(filename.FullName, true);
            Console.WriteLine("Execution Log: {0}", filename);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            _writer = new StreamWriter(new MemoryStream());
        }

        _timer = new Timer(new TimerCallback((obj) =>
        {
            lock (_locker)
            { 
                _writer.Flush();    
            }
            
        }), _locker, 50, 100);
    }

    private void Log(string message, LoggerLevel level)
    {
        if (((int)level) >= ((int)Level))
        {
            lock (_locker)
            {
                _writer.WriteLine(message);
            }

        }
    }

    public void Dispose()
    {
        _timer.Dispose();
        _writer.Flush();

        if (_writer.BaseStream is MemoryStream)
        {
            var reader = new StreamReader(_writer.BaseStream);
            reader.BaseStream.Position = 0;
            Console.WriteLine(reader.ReadToEnd());    
        }
            

        _writer.Close();
        _writer.Dispose();
    }
    
    public static Logger Instance = new Logger();
    public static bool Enabled = true;
    public static string FormatDate = "yyyy-MM-dd HH:mm:ss.ffffff";
    public static LoggerLevel Level = LoggerLevel.INFO;

    public static void LogLexer(string message, [CallerMemberName] string caller = "")
    {
        if (!Enabled) return;
        var formateDate = DateTime.UtcNow.ToString(FormatDate);
        var done = string.Format("TRA [{0}] | Lexer.{2} | {1}", formateDate, message, caller.PadRight(25, ' '));
        Instance.Log(done, LoggerLevel.TRACE);
    }

    public static void LogParse(string message, [CallerMemberName] string caller = "")
    {
        if (!Enabled) return;
        var formateDate = DateTime.UtcNow.ToString(FormatDate);
        var done = string.Format("TRA [{0}] | Parse.{2} | {1}", formateDate, message, caller.PadRight(25, ' '));
        Instance.Log(done, LoggerLevel.TRACE);
    }

    public static void LogAnalize(string message, [CallerMemberName] string caller = "")
    {
        if (!Enabled) return;
        var formateDate = DateTime.UtcNow.ToString(FormatDate);
        var done = string.Format("TRA [{0}] | Analizer.{2} | {1}", formateDate, message, caller.PadRight(22, ' '));
        Instance.Log(done, LoggerLevel.TRACE);
    }

    public static void LogSemanticContext(string message, [CallerMemberName] string caller = "")
    {
        if (!Enabled) return;
        var formateDate = DateTime.UtcNow.ToString(FormatDate);
        var done = string.Format("TRA [{0}] | SemanticContext.{2} | {1}", formateDate, message, caller.PadRight(15, ' '));
        Instance.Log(done, LoggerLevel.TRACE);
    }

    public static void LogInterpreter(string message, [CallerMemberName] string caller = "")
    {
        if (!Enabled) return;
        var formateDate = DateTime.UtcNow.ToString(FormatDate);
        var done = string.Format("DEB [{0}] | Interpreter.{2} | {1}", formateDate, message, caller.PadRight(20, ' '));
        Instance.Log(done, LoggerLevel.DEBUG);
    }
    
    public static void LogRuntime(string message, [CallerMemberName] string caller = "")
    {
        if (!Enabled) return;
        var formateDate = DateTime.UtcNow.ToString(FormatDate);
        var done = string.Format("DEB [{0}] | Scope.{2} | {1}", formateDate, message, caller.PadRight(26, ' '));
        Instance.Log(done, LoggerLevel.DEBUG);
    }

    public static void LogError(Exception exception, string message, [CallerMemberName] string caller = "")
    {
        var formateDate = DateTime.UtcNow.ToString(FormatDate);
        var done = string.Format("ERR [{0}] | Compile.{2} | {1}\n{3}", formateDate, message, caller.PadRight(23, ' '), exception);
        Instance.Log(done, LoggerLevel.ERROR);
    }

}
