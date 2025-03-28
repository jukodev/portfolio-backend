using System.Diagnostics;

namespace portfolio_backend.Middlewares;

public class LoggingMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        await next(context);

        stopwatch.Stop();
        var responseTime = stopwatch.ElapsedMilliseconds;
        var statusCode = context.Response.StatusCode;
        var method = context.Request.Method;
        var path = context.Request.Path;

        var logMessage = $"{method} {path} - {statusCode} - {responseTime}ms";

        Console.WriteLine(logMessage);
    }
}