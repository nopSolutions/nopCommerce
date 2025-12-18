using Nop.Core.Domain.Orders;

namespace Nop.Plugin.Theme.KungFu.Services;

/// <summary>
/// AI Sage service interface for generating philosophical messages about orders
/// </summary>
public interface IAISageService
{
    /// <summary>
    /// Generate a sage message about an order using AI
    /// </summary>
    /// <param name="order">The order to generate a message about</param>
    /// <returns>A philosophical message from a Chinese sage</returns>
    Task<string> GenerateSageMessageAsync(Order order);

    /// <summary>
    /// Check if AI sage service is configured and ready
    /// </summary>
    /// <returns>True if configured, false otherwise</returns>
    Task<bool> IsConfiguredAsync();
}
