using Microsoft.AspNetCore.Mvc;
using Nop.Core.Configuration;
using Nop.Core.Domain.Events;
using Nop.Data;

namespace Nop.Web.Controllers;

//do not inherit it from BasePublicController — health probes should be lightweight
[Route("integration/health")]
public partial class IntegrationHealthController : Controller
{
    private readonly IRepository<IntegrationEvent> _integrationEventRepository;
    private readonly IntegrationConfig _integrationConfig;

    public IntegrationHealthController(
        IRepository<IntegrationEvent> integrationEventRepository,
        IntegrationConfig integrationConfig)
    {
        _integrationEventRepository = integrationEventRepository;
        _integrationConfig = integrationConfig;
    }

    [HttpGet("")]
    public virtual async Task<IActionResult> Index()
    {
        var pending = await _integrationEventRepository.GetAllAsync(
            q => q.Where(e => !e.Published));

        var lastPublished = await _integrationEventRepository.GetAllAsync(
            q => q.Where(e => e.Published && e.PublishedOnUtc != null)
                  .OrderByDescending(e => e.PublishedOnUtc));

        var lastPublishedOnUtc = lastPublished.FirstOrDefault()?.PublishedOnUtc;
        var ageSeconds = lastPublishedOnUtc.HasValue
            ? (DateTime.UtcNow - lastPublishedOnUtc.Value).TotalSeconds
            : (double?)null;

        var degraded = pending.Count > 0 &&
            (!ageSeconds.HasValue || ageSeconds > _integrationConfig.HealthDegradedAfterSeconds);

        var payload = new
        {
            status = degraded ? "degraded" : "healthy",
            pendingOutboxCount = pending.Count,
            lastPublishedOnUtc,
            lastPublishedAgeSeconds = ageSeconds,
            degradedAfterSeconds = _integrationConfig.HealthDegradedAfterSeconds
        };

        return StatusCode(degraded ? StatusCodes.Status503ServiceUnavailable : StatusCodes.Status200OK, payload);
    }
}
