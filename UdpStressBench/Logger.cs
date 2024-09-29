using System.Collections.Concurrent;
using System.Collections.ObjectModel;
// ReSharper disable CheckNamespace

public class Logger : IDisposable
{
    public enum LogType
    {
        Info,
        Warning,
        Error,
    }

    private readonly ConcurrentQueue<(LogType, DateTimeOffset, string)> _logs = new();

    public ReadOnlyCollection<(LogType, DateTimeOffset, string)> GetLogs()
    {
        var res = new List<(LogType, DateTimeOffset, string)>(_logs);
        return res.AsReadOnly();
    }
    
    public void Log(string message) => PostLog(LogType.Info, message);
    public void LogWarning(string message) => PostLog(LogType.Warning, message);
    public void LogError(string message) => PostLog(LogType.Error, message);

    private void PostLog(LogType type, string message)
    {
        var ts = DateTimeOffset.Now;
        
        switch (type)
        {
            case LogType.Warning:
                Console.ForegroundColor = ConsoleColor.Yellow;
                break;
            case LogType.Error:
                Console.ForegroundColor = ConsoleColor.Red;
                break;
            case LogType.Info:
            default:
                Console.ResetColor();
                break;
        }
            
        var jst = TimeZoneInfo.ConvertTimeFromUtc(ts.UtcDateTime, TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time"));
        var timeStr = $"{jst:yyyy-MM-dd HH:mm:ss}";
        var typeStr = type switch
        {
            LogType.Info => "Info",
            LogType.Warning => "Warning",
            LogType.Error => "Error",
            _ => "Unknown",
        };
        Console.WriteLine($"{timeStr} [{typeStr}] {message}");
    }
    
    public void Dispose()
    {
        _logs.Clear();
    }
}