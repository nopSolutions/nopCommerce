using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Integration.Pos.Models;
using Nop.Plugin.Inventory.AllocationGate;
using Nop.Plugin.Inventory.AllocationGate.Services;
using Nop.Services.Configuration;
using Nop.Services.Stores;

namespace Nop.Plugin.Integration.Pos.Controllers;

[ApiController]
[Route("api/inventory")]
public class AllocationApiController : ControllerBase
{
    private readonly IAllocationGate _gate;
    private readonly ISettingService _settingService;
    private readonly IStoreContext _storeContext;

    public AllocationApiController(IAllocationGate gate, ISettingService settingService, IStoreContext storeContext)
    {
        _gate = gate;
        _settingService = settingService;
        _storeContext = storeContext;
    }

    [HttpPost("reserve")]
    public async Task<IActionResult> Reserve([FromBody] ReserveRequest request)
    {
        if (!await IsAuthorizedAsync())
            return Unauthorized(new { error = "invalid-api-key" });

        if (request.ProductId <= 0 || request.Quantity <= 0 || string.IsNullOrWhiteSpace(request.ReservationKey))
            return BadRequest(new { error = "invalid-request" });

        var result = await _gate.ReserveAsync(
            request.ProductId,
            request.WarehouseId,
            request.Quantity,
            "pos",
            request.ReservationKey,
            request.TtlSeconds);

        return result.Success
            ? Ok(new { reservationKey = request.ReservationKey, message = result.Message })
            : Conflict(new { error = result.Message });
    }

    [HttpPost("confirm")]
    public async Task<IActionResult> Confirm([FromBody] ConfirmRequest request)
    {
        if (!await IsAuthorizedAsync())
            return Unauthorized(new { error = "invalid-api-key" });

        if (string.IsNullOrWhiteSpace(request?.ReservationKey))
            return BadRequest(new { error = "invalid-request" });

        var ok = await _gate.ConfirmAsync(request.ReservationKey);
        return ok ? Ok(new { confirmed = true }) : NotFound(new { error = "reservation-not-found" });
    }

    [HttpPost("release")]
    public async Task<IActionResult> Release([FromBody] ReleaseRequest request)
    {
        if (!await IsAuthorizedAsync())
            return Unauthorized(new { error = "invalid-api-key" });

        if (string.IsNullOrWhiteSpace(request?.ReservationKey))
            return BadRequest(new { error = "invalid-request" });

        var ok = await _gate.ReleaseAsync(request.ReservationKey);
        return ok ? Ok(new { released = true }) : NotFound(new { error = "reservation-not-found" });
    }

    private async Task<bool> IsAuthorizedAsync()
    {
        var store = await _storeContext.GetCurrentStoreAsync();
        var settings = await _settingService.LoadSettingAsync<AllocationSettings>(store.Id);
        var apiKey = Request.Headers["X-Api-Key"].FirstOrDefault()
                     ?? Request.Query["apiKey"].FirstOrDefault();
        return apiKey == settings.PosApiKey;
    }
}
