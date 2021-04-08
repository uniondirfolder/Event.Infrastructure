using System;
using System.Text;
using Murmur;
using Serilog.Core;
using Serilog.Events;

namespace Event.Infrastructure.Host.Logs.Enrichers
{
    /// <summary>
    /// Add unique identity for logs of the same type. 
    /// Source code copied from:  https://benfoster.io/blog/serilog-best-practices/.
    /// </summary>
    public class EventTypeEnricher : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            if (logEvent is null)
                throw new ArgumentNullException(nameof(logEvent));

            if (propertyFactory is null)
                throw new ArgumentNullException(nameof(propertyFactory));

            Murmur32 murmur = MurmurHash.Create32();
            byte[] bytes = Encoding.UTF8.GetBytes(logEvent.MessageTemplate.Text);
            byte[] hash = murmur.ComputeHash(bytes);
            string hexadecimalHash = BitConverter.ToString(hash).Replace("-", "");
            LogEventProperty eventId = propertyFactory.CreateProperty("EventType", hexadecimalHash);
            logEvent.AddPropertyIfAbsent(eventId);
        }
    }
}