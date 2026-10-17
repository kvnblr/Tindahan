using System.Text.Json;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Api.Health;

public static class HealthResponseWriter
{
    public static async Task WriteResponse(HttpContext context, HealthReport report)
    {
        context.Response.ContentType = "application/json";

        var response = new
        {
            status = report.Status.ToString(),
            totalDuration = Math.Round(report.TotalDuration.TotalMilliseconds, 2),
            checks = report.Entries.Select(entry => new
            {
                name = entry.Key,
                status = entry.Value.Status.ToString(),
                duration = Math.Round(entry.Value.Duration.TotalMilliseconds, 2)
            })
        };

        await JsonSerializer
            .SerializeAsync(context.Response.Body, response, cancellationToken: context.RequestAborted);
    }
}
