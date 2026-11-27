namespace Bison.Razor;

public partial class BisonLogger
{
    [LoggerMessage(Level = LogLevel.Trace, Message = "Got {method} request for {route}, with params {args}")]
    public static partial void LogRequestWithParams(ILogger logger, string method, string route, object?[] args);

    [LoggerMessage(Level = LogLevel.Trace, Message = "Returning page")]
    public static partial void ReturningPage(ILogger logger);

    [LoggerMessage(Level = LogLevel.Trace, Message = "Invalid taxon id '{taxon}' is given, returning page")]
    public static partial void LogInvalidTaxon(ILogger logger, string taxon);

    [LoggerMessage(Level = LogLevel.Trace, Message = "Input model is valid")]
    public static partial void LogIsValidModel(ILogger logger);
}
