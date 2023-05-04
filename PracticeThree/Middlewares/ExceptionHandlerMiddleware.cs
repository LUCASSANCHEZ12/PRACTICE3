using System.Globalization;

namespace UPB.PracticeThree.Middlewares;

public class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly Serilog.ILogger _log;

    public ExceptionHandlerMiddleware(RequestDelegate next, Serilog.ILogger logger)
    {
        _next = next;
        _log = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // Call the next delegate/middleware in the pipeline.
            _log.Information("Request succes");
            await _next(context);
        }
        catch (System.Exception ex)
        {
            _log.Error("Request faild: " + ex.Message);
            HandleException(context, ex);
        }
    }

    private static Task HandleException(HttpContext context, Exception ex)
    {
        context.Response.ContentType = "text/json";
        //context.Response.StatusCode = 500; 
        return context.Response.WriteAsync("Sucedio el siguiente error: "+ex.Message);
    }
}
public static class ExceptionHandlerExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app, Serilog.ILogger logger)
    {
        return app.UseMiddleware<ExceptionHandlerMiddleware>(logger);
    }
}