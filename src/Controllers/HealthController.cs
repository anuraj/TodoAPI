using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace TodoApi.Controllers
{
    [ExcludeFromCodeCoverage]
    [ApiController]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [ApiVersion("3.0")]
    [Route("{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    [ApiConventionType(typeof(DefaultApiConventions))]
    public class HealthController : ControllerBase
    {
        private readonly ILogger<HealthController> _logger;
        private readonly HealthCheckService _healthCheckService;
        public HealthController(ILogger<HealthController> logger,
            HealthCheckService healthCheckService)
        {
            _healthCheckService = healthCheckService;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(503)]
        public async Task<IActionResult> Get()
        {
            var report = await _healthCheckService.CheckHealthAsync();
            return report.Status == HealthStatus.Healthy ? Ok(report) :
                StatusCode((int)HttpStatusCode.ServiceUnavailable, report);
        }
    }
}