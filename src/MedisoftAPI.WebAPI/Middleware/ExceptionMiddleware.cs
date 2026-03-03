using System.Net;
using System.Text.Json;
using MedisoftAPI.Application.DTOs;

namespace MedisoftAPI.WebAPI.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate              _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly IWebHostEnvironment          _env;

    public ExceptionMiddleware(RequestDelegate next,
        ILogger<ExceptionMiddleware> logger,
        IWebHostEnvironment env)
    {
        _next   = next;
        _logger = logger;
        _env    = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Recurso no encontrado");
            await WriteAsync(context, HttpStatusCode.NotFound, ex.Message, ex);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "No autorizado");
            await WriteAsync(context, HttpStatusCode.Unauthorized, ex.Message, ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error no controlado: {Message}", ex.Message);

            // En Development muestra el error completo para facilitar diagnóstico
            var message = _env.IsDevelopment()
                ? $"{ex.GetType().Name}: {ex.Message} | {ex.InnerException?.Message}"
                : "Error interno del servidor. Contacte al administrador.";

            await WriteAsync(context, HttpStatusCode.InternalServerError, message, ex);
        }
    }

    private static Task WriteAsync(HttpContext ctx, HttpStatusCode code, string msg, Exception? ex = null)
    {
        ctx.Response.ContentType = "application/json";
        ctx.Response.StatusCode  = (int)code;
        var json = JsonSerializer.Serialize(
            ApiResponse<string>.Fail(msg),
            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        return ctx.Response.WriteAsync(json);
    }
}
