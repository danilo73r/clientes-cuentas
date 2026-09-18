using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Shared.Domain.Exceptions;

namespace Shared.Api.Exceptions;

public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (status, title) = exception switch
        {
            ReglaNegocioException => (403, "Error"),
            ValidacionException => (400, "Datos inválidos"),
            NoEncontradoException => (404, "No se encontró"),
            ConflictoException => (409, "Conflicto"),
            _ => (500, "Error interno")
        };

        if (status == 500)
            logger.LogError(exception,
                "Error interno. TraceId: {TraceId}",
                httpContext.TraceIdentifier);

        var problema = new ProblemDetails
        {
            Status = status,
            Title = title,
            Detail = status == 500
                ? "Sucedió un error interno inesperado."
                : exception.Message
        };

        if (exception is ReglaNegocioException regla)
            problema.Extensions["code"] = regla.Codigo;

        httpContext.Response.StatusCode = status;

        await httpContext.Response.WriteAsJsonAsync(
            problema,
            options: null,
            contentType: "application/problem+json",
            cancellationToken: cancellationToken);

        return true;
    }
}