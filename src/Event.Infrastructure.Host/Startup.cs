using System.Text.Json;
using Event.Infrastructure.Host.Health;
using Event.Infrastructure.Host.Logs;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Event.Infrastructure.Host
{
    public class Startup
    {
        private const string DbConfigKey = "Db";
        private const string RedisConfigKey = "Redis";
        private const string MemoryHealthCheckName = "memory_health";

        private const string ReadinessTag = "ready";
        private const string LivenessTag = "live";
        
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        
        private IConfiguration Configuration { get; }
        
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvcCore();
            services.AddRouting(options => options.LowercaseUrls = true);
            services.AddControllersWithViews()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
                });

            var seqConf = new SeqOptions();
            Configuration.GetSection(nameof(SeqOptions)).Bind(seqConf);
            
            services.AddHealthChecks()
                .AddMemoryHealthCheck(MemoryHealthCheckName, tags: new[] { LivenessTag }) // Custom health check registration
                .AddMySql(Configuration.GetConnectionString(DbConfigKey), tags: new[] { ReadinessTag })
                .AddRedis(Configuration.GetConnectionString(RedisConfigKey), failureStatus: HealthStatus.Degraded, tags: new[] { ReadinessTag })
                .AddSeqPublisher(t =>
                {
                    t.Endpoint = seqConf.Endpoint;
                    t.ApiKey = seqConf.ApiKey;
                    t.DefaultInputLevel = seqConf.DefaultInputLevel;
                });
        }
        
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.UseSerilogRequestLogging(opts
                => opts.EnrichDiagnosticContext = LogEnricher.EnrichFromRequest);
            
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/api/healthz", new HealthCheckOptions
                {
                    Predicate = (check) => check.Tags.Contains(ReadinessTag),
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });
                endpoints.MapHealthChecks("/api/healthz/live", new HealthCheckOptions
                {
                    Predicate = (check) => check.Tags.Contains(LivenessTag),
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });
            });

            app.UseHealthChecksPrometheusExporter("/api/health-metrics");
        }
    }
}