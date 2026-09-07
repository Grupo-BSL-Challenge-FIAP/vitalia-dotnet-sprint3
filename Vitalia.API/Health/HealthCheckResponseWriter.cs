using System.Text.Json;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Vitalia.API.Health;

public static class HealthCheckResponseWriter
{
    public static Task WriteJsonResponse(
        HttpContext context,
        HealthReport report)
    {
        context.Response.ContentType = "application/json";

        var payload = new
        {
            status = report.Status.ToString(),
            duration = report.TotalDuration,

            checks = report.Entries.Select(entry => new
            {
                name = entry.Key,
                status = entry.Value.Status.ToString(),
                description = entry.Value.Description,
                duration = entry.Value.Duration,
                error = entry.Value.Exception?.Message
            })
        };

        return context.Response.WriteAsync(
            JsonSerializer.Serialize(payload)
        );
    }
}