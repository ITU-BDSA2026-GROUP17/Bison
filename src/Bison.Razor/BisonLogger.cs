namespace Bison.Razor;

public partial class BisonLogger
{
    [LoggerMessage(Level = LogLevel.Trace, Message = "Got {method} request for {route}, with params {args}")]
    public static partial void LogRequestWithParams(ILogger logger, string method, string route, object?[] args);

    [LoggerMessage(Level = LogLevel.Trace, Message = "Returning page")]
    public static partial void ReturningPage(ILogger logger);
}
