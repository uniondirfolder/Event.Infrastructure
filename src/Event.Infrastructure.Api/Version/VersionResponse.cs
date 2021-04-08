namespace Event.Infrastructure.Api.Version
{
    public class VersionResponse
    {
        public string Version { get; set; } = default!;
        
        public string InfoVersion { get; set; } = default!;
        
        public string Product { get; set; } = default!;
    }
}