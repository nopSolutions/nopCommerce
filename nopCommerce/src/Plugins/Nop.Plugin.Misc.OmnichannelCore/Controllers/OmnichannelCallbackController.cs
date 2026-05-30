using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Nop.Plugin.Misc.OmnichannelCore.Models.Callbacks;
using Nop.Plugin.Misc.OmnichannelCore.Services;

namespace Nop.Plugin.Misc.OmnichannelCore.Controllers;

[IgnoreAntiforgeryToken]
[Route("omnichannel/callbacks")]
public class OmnichannelCallbackController : Controller
{
    #region Fields

    private readonly IConfiguration _configuration;
    private readonly OmniInboxService _omniInboxService;
    private readonly OmniStockSyncService _omniStockSyncService;

    #endregion

    #region Ctor

    public OmnichannelCallbackController(IConfiguration configuration,
        OmniInboxService omniInboxService,
        OmniStockSyncService omniStockSyncService)
    {
        _configuration = configuration;
        _omniInboxService = omniInboxService;
        _omniStockSyncService = omniStockSyncService;
    }

    #endregion

    #region Methods

    [HttpPost("pos/stock-changed")]
    public virtual async Task<IActionResult> PosStockChanged([FromBody] PosStockChangedRequest request)
    {
        if (!IsAuthorized())
            return Unauthorized(new OmnichannelCallbackResponse
            {
                Result = "unauthorized",
                Detail = $"Missing or invalid {OmnichannelCoreDefaults.DemoTokenHeaderName} header"
            });

        var validationError = Validate(request);
        if (validationError != null)
            return BadRequest(new OmnichannelCallbackResponse
            {
                Result = "invalid",
                MessageId = request?.MessageId ?? Guid.Empty,
                CorrelationId = request?.CorrelationId,
                Detail = validationError
            });

        var inboxResult = await _omniInboxService.TryBeginProcessingAsync(new InboxMessageContext
        {
            MessageId = request.MessageId,
            EventType = request.EventType,
            CorrelationId = request.CorrelationId,
            Source = request.Source,
            OrderGuid = null
        });

        if (inboxResult.IsDuplicate)
            return Ok(new OmnichannelCallbackResponse
            {
                Result = "duplicate",
                MessageId = request.MessageId,
                CorrelationId = request.CorrelationId,
                InboxId = inboxResult.InboxMessage?.Id,
                Duplicate = true,
                Detail = "messageId already exists in OmniInboxMessage"
            });

        try
        {
            var stockResult = await _omniStockSyncService.ApplyPosStockChangedAsync(request);

            await _omniInboxService.MarkProcessedAsync(inboxResult.InboxMessage);

            return Ok(new OmnichannelCallbackResponse
            {
                Result = stockResult.Result,
                MessageId = request.MessageId,
                CorrelationId = request.CorrelationId,
                InboxId = inboxResult.InboxMessage.Id,
                StockSyncStateId = stockResult.StockSyncState.Id,
                Applied = stockResult.Applied,
                Duplicate = false,
                Stale = stockResult.Stale,
                SourceVersion = stockResult.StockSyncState.SourceVersion,
                Detail = stockResult.Detail
            });
        }
        catch (Exception exception)
        {
            await _omniInboxService.MarkFailedAsync(inboxResult.InboxMessage, exception.Message);

            return StatusCode(500, new OmnichannelCallbackResponse
            {
                Result = "failed",
                MessageId = request.MessageId,
                CorrelationId = request.CorrelationId,
                InboxId = inboxResult.InboxMessage.Id,
                Detail = exception.Message
            });
        }
    }

    #endregion

    #region Utilities

    private bool IsAuthorized()
    {
        if (!Request.Headers.TryGetValue(OmnichannelCoreDefaults.DemoTokenHeaderName, out StringValues submittedToken))
            return false;

        var expectedToken = _configuration["OmnichannelCore:DemoToken"] ?? OmnichannelCoreDefaults.DefaultDemoToken;

        return string.Equals(submittedToken.ToString(), expectedToken, StringComparison.Ordinal);
    }

    private static string Validate(PosStockChangedRequest request)
    {
        if (request == null)
            return "Request body is required";

        if (request.MessageId == Guid.Empty)
            return "messageId is required";

        if (string.IsNullOrWhiteSpace(request.EventType))
            return "eventType is required";

        if (!string.Equals(request.EventType, OmnichannelCoreDefaults.PosStockChangedEventType, StringComparison.Ordinal))
            return $"eventType must be {OmnichannelCoreDefaults.PosStockChangedEventType}";

        if (request.ProductId <= 0)
            return "productId must be greater than zero";

        if (request.WarehouseId < 0)
            return "warehouseId cannot be negative";

        if (request.SourceVersion < 0)
            return "sourceVersion cannot be negative";

        return null;
    }

    #endregion
}
