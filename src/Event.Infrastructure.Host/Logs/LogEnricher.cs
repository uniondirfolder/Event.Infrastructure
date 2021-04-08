using System;
using System.Linq;
using Event.Infrastructure.Host.Logs.Extensions;
using Microsoft.AspNetCore.Http;
using Serilog;

namespace Event.Infrastructure.Host.Logs
{
    public static class LogEnricher
    {
        /// <summary>
        /// Enriches the HTTP request log with additional data via the Diagnostic Context.
        /// Source code copied from:  https://benfoster.io/blog/serilog-best-practices/.
        /// </summary>
        /// <param name="diagnosticContext">The Serilog diagnostic context.</param>
        /// <param name="httpContext">The current HTTP Context.</param>
        public static void EnrichFromRequest(IDiagnosticContext diagnosticContext, HttpContext httpContext)
        {
            diagnosticContext.Set("ClientIP", httpContext.Connection.RemoteIpAddress?.ToString());
            diagnosticContext.Set("UserAgent", httpContext.Request.Headers["User-Agent"].FirstOrDefault());
            diagnosticContext.Set("Resource", httpContext.GetMetricsCurrentResourceName());
            diagnosticContext.Set("Environment", Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"));
        }
    }
}