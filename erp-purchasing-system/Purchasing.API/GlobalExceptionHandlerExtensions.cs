using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

public static class GlobalExceptionHandlerExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
    {
        return app.Use(async (context, next) =>
        {
            try
            {
                await next();
            }
            catch (Exception exception)
            {
                var logger = context.RequestServices
                    .GetRequiredService<ILoggerFactory>()
                    .CreateLogger("GlobalExceptionHandler");

                if (context.Response.HasStarted)
                {
                    logger.LogError(exception, "Unhandled exception occurred after the response started.");
                    throw;
                }

                await WriteProblemDetailsAsync(context, exception, logger);
            }
        });
    }

    private static async Task WriteProblemDetailsAsync(
        HttpContext context,
        Exception exception,
        ILogger logger)
    {
        var (statusCode, title, detail) = MapException(exception);

        logger.LogError(exception, "Unhandled exception returned as ProblemDetails.");

        context.Response.Clear();
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json";

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = context.Request.Path
        };

        problemDetails.Extensions["traceId"] = Activity.Current?.Id ?? context.TraceIdentifier;

        await context.Response.WriteAsJsonAsync(problemDetails);
    }

    private static (int StatusCode, string Title, string Detail) MapException(Exception exception)
    {
        return exception switch
        {
            KeyNotFoundException => (
                StatusCodes.Status404NotFound,
                "Recurso no encontrado",
                GetExceptionMessage(exception, "El recurso solicitado no fue encontrado.")),

            ArgumentException => (
                StatusCodes.Status400BadRequest,
                "Solicitud invalida",
                GetExceptionMessage(exception, "La solicitud no es valida.")),

            InvalidOperationException => (
                StatusCodes.Status400BadRequest,
                "Operacion invalida",
                GetExceptionMessage(exception, "La operacion solicitada no es valida.")),

            UnauthorizedAccessException => (
                StatusCodes.Status403Forbidden,
                "Acceso denegado",
                GetExceptionMessage(exception, "No tienes permisos para realizar esta accion.")),

            _ => (
                StatusCodes.Status500InternalServerError,
                "Error interno del servidor",
                "Ocurrio un error inesperado. Intenta nuevamente o contacta a soporte.")
        };
    }

    private static string GetExceptionMessage(Exception exception, string fallbackMessage)
    {
        return string.IsNullOrWhiteSpace(exception.Message)
            ? fallbackMessage
            : exception.Message;
    }
}
