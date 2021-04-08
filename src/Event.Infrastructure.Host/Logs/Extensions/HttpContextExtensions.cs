using System;
using Event.Infrastructure.Host.Logs.Enrichers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Serilog;
using Serilog.Configuration;

namespace Event.Infrastructure.Host.Logs.Extensions
{
    /// <summary>
    /// Source code copied from:  https://benfoster.io/blog/serilog-best-practices/.
    /// </summary>
    public static class HttpContextExtensions
    {
        /// <summary>
        /// Gets the current API resource name from HTTP context.
        /// </summary>
        /// <param name="httpContext">The HTTP context.</param>
        /// <returns>The current resource name if available, otherwise an empty string.</returns>
        public static string? GetMetricsCurrentResourceName(this HttpContext httpContext)
        {
            Endpoint? endpoint = httpContext!.GetEndpoint();
            return endpoint?.Metadata.GetMetadata<EndpointNameMetadata>()?.EndpointName;
        }
        
        /// <summary>
        /// Registers event enricher during the configuration of logging.
        /// </summary>
        /// <param name="enrich">Config param for enricher.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">enrich could not be empty.</exception>
        public static LoggerConfiguration WithEventType(this LoggerEnrichmentConfiguration enrich)
        {
            if (enrich == null)
                throw new ArgumentNullException(nameof(enrich));

            return enrich.With<EventTypeEnricher>();
        }
    }
}