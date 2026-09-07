using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Vitalia.API.Exceptions;

public sealed class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger,
    IHostEnvironment environment) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var traceId =
            System.Diagnostics.Activity.Current?
                .TraceId
                .ToString()
            ?? httpContext.TraceIdentifier;

        var (statusCode, title, detail) =
            MapException(
                exception,
                environment);

        LogException(
            exception,
            statusCode,
            traceId);

        httpContext.Response.StatusCode =
            statusCode;

        httpContext.Response.ContentType =
            "application/problem+json";

        var problem = new ProblemDetails
        {
            Type =
                $"https://httpstatuses.com/{statusCode}",

            Title =
                title,

            Status =
                statusCode,

            Detail =
                detail,

            Instance =
                httpContext.Request.Path
        };

        if (environment.IsDevelopment())
        {
            problem.Extensions["traceId"] =
                traceId;
        }

        await httpContext.Response.WriteAsJsonAsync(
            problem,
            cancellationToken);

        return true;
    }

    private void LogException(
        Exception exception,
        int statusCode,
        string traceId)
    {
        if (statusCode >=
            StatusCodes.Status500InternalServerError)
        {
            logger.LogError(
                exception,
                "Erro inesperado durante processamento da requisição. " +
                "StatusCode: {StatusCode} | " +
                "ExceptionType: {ExceptionType} | " +
                "Message: {Message} | " +
                "TraceId: {TraceId}",
                statusCode,
                exception.GetType().Name,
                exception.Message,
                traceId);

            return;
        }

        logger.LogWarning(
            exception,
            "Falha esperada durante processamento da requisição. " +
            "StatusCode: {StatusCode} | " +
            "ExceptionType: {ExceptionType} | " +
            "Message: {Message} | " +
            "TraceId: {TraceId}",
            statusCode,
            exception.GetType().Name,
            exception.Message,
            traceId);
    }

    private static (
        int StatusCode,
        string Title,
        string Detail)
        MapException(
            Exception exception,
            IHostEnvironment environment)
    {
        return exception switch
        {
            KeyNotFoundException e =>
                (
                    StatusCodes.Status404NotFound,
                    "Recurso não encontrado.",
                    e.Message
                ),

            ArgumentException e =>
                (
                    StatusCodes.Status400BadRequest,
                    "Dados inválidos.",
                    e.Message
                ),

            InvalidOperationException e =>
                (
                    StatusCodes.Status409Conflict,
                    "Conflito de regra de negócio.",
                    e.Message
                ),

            UnauthorizedAccessException e =>
                (
                    StatusCodes.Status403Forbidden,
                    "Acesso negado.",
                    e.Message
                ),

            DbUpdateException e
                when IsForeignKeyViolation(e) =>
                (
                    StatusCodes.Status409Conflict,
                    "Conflito de regra de negócio.",
                    "Não é possível excluir a categoria porque existem produtos associados a ela."
                ),

            _ =>
                MapUnhandled(
                    environment,
                    exception)
        };
    }

    private static (
        int StatusCode,
        string Title,
        string Detail)
        MapUnhandled(
            IHostEnvironment environment,
            Exception exception)
    {
        if (environment.IsDevelopment())
        {
            return (
                StatusCodes.Status500InternalServerError,
                "Erro interno do servidor.",
                exception.Message
            );
        }

        return (
            StatusCodes.Status500InternalServerError,
            "Erro interno do servidor.",
            "Ocorreu um erro inesperado. Tente novamente mais tarde."
        );
    }

    private static bool IsForeignKeyViolation(
        DbUpdateException exception)
    {
        return exception.InnerException?
            .Message
            .Contains(
                "ORA-02292",
                StringComparison.OrdinalIgnoreCase)
            == true;
    }
}