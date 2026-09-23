using FluentValidation;
using System.Text.Json;
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
            ValidationException => (400, "Datos inválidos"), // FluentValidation
            ValidacionException => (400, "Datos inválidos"),
            NoEncontradoException => (404, "No se encontró"),
            ConflictoException => (409, "Conflicto"),
            BadHttpRequestException req => (req.StatusCode, "Solicitud inválida"),
            _ => (500, "Error interno")
        };

        if (status == 500)
            logger.LogError(exception,
                "Error interno. TraceId: {TraceId}",
                httpContext.TraceIdentifier);

        var problema = CreateProblema(exception, status, title);

        problema.Extensions["traceId"] = httpContext.TraceIdentifier;
        if (exception is ReglaNegocioException regla)
            problema.Extensions["code"] = regla.Codigo;


        httpContext.Response.StatusCode = status;

        await httpContext.Response.WriteAsJsonAsync<object>(
            problema,
            options: null,
            contentType: "application/problem+json",
            cancellationToken: cancellationToken);

        return true;
    }

    private static ProblemDetails CreateProblema(
    Exception exception, int status, string title)
    {
        if (exception is ValidationException validacion && validacion.Errors.Any())
        {
            var errores = validacion.Errors
                .GroupBy(error =>
                    JsonNamingPolicy.CamelCase.ConvertName(error.PropertyName))
                .ToDictionary(
                    grupo => grupo.Key,
                    grupo => grupo.Select(error => error.ErrorMessage)
                                  .Distinct()
                                  .ToArray());

            return new ValidationProblemDetails(errores)
            {
                Status = status,
                Title = title
            };
        }

        return new ProblemDetails
        {
            Status = status,
            Title = title,
            Detail = GetProblemDetail(exception, status)
        };
    }

    private static string GetProblemDetail(Exception exception, int status)
    {
        if (exception is BadHttpRequestException)
            return "No se pudo interpretar la solicitud";

        if (status == 500)
            return "Sucedió un error interno inesperado";

        return exception.Message;
    }
}
