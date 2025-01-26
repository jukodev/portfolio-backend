namespace portfolio_backend.MIddlewares;

public class LoggingMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        await next(context);
        var statusCode = context.Response.StatusCode;
        var method = context.Request.Method;
        var path = context.Request.Path;
        var ip = context.Connection.RemoteIpAddress;

        Console.WriteLine(statusCode >= 400
            ? $"{method} {path} \x1b[31m{statusCode}\x1b[0m from {ip}"
            : $"{method} {path} \x1b[32m{statusCode}\x1b[0m from {ip}");
    }
}