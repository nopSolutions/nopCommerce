using Microsoft.Extensions.Logging;
using Nop.Core.Domain.Orders;
using Nop.Core.Events;
using Nop.Plugin.Theme.KungFu.Services;
using Nop.Services.Events;
using Nop.Services.Messages;


namespace Nop.Plugin.Theme.KungFu.Infrastructure;

/// <summary>
/// Event consumer for order events to add AI sage message tokens
/// </summary>
public class OrderTokenEventConsumer : IConsumer<EntityTokensAddedEvent<Order>>
{
    private readonly ILogger<OrderTokenEventConsumer> _logger;
    private readonly IAISageService _aiSageService;

    public OrderTokenEventConsumer(
        ILogger<OrderTokenEventConsumer> logger,
        IAISageService aiSageService)
    {
        _logger = logger;
        _aiSageService = aiSageService;
    }

    /// <summary>
    /// Handle entity tokens added event to inject AI sage message token
    /// </summary>
    public async Task HandleEventAsync(EntityTokensAddedEvent<Order> eventMessage)
    {
        if (eventMessage?.Entity == null || eventMessage.Tokens == null)
            return;

        try
        {
            // Check if AI sage service is configured
            if (!await _aiSageService.IsConfiguredAsync())
            {
                // Add a default wisdom token even if AI is not configured
                eventMessage.Tokens.Add(new Token("AI.SageMessage", 
                    "Your order brings honor to your practice. May the path of discipline lead you to inner peace and strength."));
                return;
            }

            // Generate the sage message using AI
            var sageMessage = await _aiSageService.GenerateSageMessageAsync(eventMessage.Entity);

            // Add the token
            eventMessage.Tokens.Add(new Token("AI.SageMessage", sageMessage));

            _logger.LogDebug("AI Sage message token added for order {OrderId}", eventMessage.Entity.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding AI sage token for order {OrderId}", eventMessage.Entity?.Id ?? 0);
            // Add a fallback token so the template doesn't break
            eventMessage.Tokens.Add(new Token("AI.SageMessage", 
                "Your order reflects your commitment to the path of mastery. Train with dedication, rest with peace."));
        }
    }
}
