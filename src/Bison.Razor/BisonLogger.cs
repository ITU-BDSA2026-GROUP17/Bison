namespace Bison.Razor;

public partial class BisonLogger
{
    [LoggerMessage(Level = LogLevel.Trace, Message = "Got request for {route}, with params {args}")]
    public static partial void LogRequestWithParams(ILogger logger, string route, object?[] args);

    [LoggerMessage(Level = LogLevel.Trace, Message = "Returning page")]
    public static partial void ReturningPage(ILogger logger);
}
