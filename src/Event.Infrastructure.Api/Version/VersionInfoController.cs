using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Event.Infrastructure.Api.Version
{
    [Route("api/version")]
    public class VersionInfoController : ControllerBase
    {
        private readonly ILogger _logger;
        
        public VersionInfoController(ILogger<VersionInfoController> logger)
        {
            _logger = logger;
        }
        
        [HttpGet]
        public ActionResult<VersionResponse> GetVersion()
        {
            Assembly currentAssembly = Assembly.GetAssembly(typeof(VersionInfoController)) !;
            var product = (AssemblyProductAttribute)currentAssembly.GetCustomAttributes(typeof(AssemblyProductAttribute)).First();
            var version = (AssemblyFileVersionAttribute)currentAssembly.GetCustomAttributes(typeof(AssemblyFileVersionAttribute)).First();
            var infoVersion = (AssemblyInformationalVersionAttribute)currentAssembly.GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute)).First();

            _logger.LogInformation("Version {Version} of the application {Product} was fetched", version.Version, product.Product);
            
            var response = new VersionResponse()
            {
                Product = product.Product,
                Version = version.Version,
                InfoVersion = infoVersion.InformationalVersion
            };
            
            return Ok(response);
        }
    }
}