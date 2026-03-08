using Nop.Plugin.Payments.Momo.Models;

namespace Nop.Plugin.Payments.Momo.Services;

/// <summary>
/// MTN MoMo payment service interface
/// </summary>
public interface IMomoPaymentService
{
    /// <summary>
    /// Gets the configuration model
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the configuration model</returns>
    Task<ConfigurationModel> GetConfigurationModelAsync();

    /// <summary>
    /// Saves the configuration
    /// </summary>
    /// <param name="model">Configuration model</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task SaveConfigurationAsync(ConfigurationModel model);

    /// <summary>
    /// Creates an API user
    /// </summary>
    /// <param name="apiUser">API user</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the result</returns>
    Task<(bool Success, string Message)> CreateApiUserAsync(string apiUser);

    /// <summary>
    /// Generates an API key
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the result</returns>
    Task<(bool Success, string Message)> GenerateApiKeyAsync();

    /// <summary>
    /// Initiates a payment
    /// </summary>
    /// <param name="phoneNumber">Phone number</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the reference ID and success status</returns>
    Task<(bool Success, string ReferenceId, string Message)> InitiatePaymentAsync(string phoneNumber);

    /// <summary>
    /// Checks the payment status
    /// </summary>
    /// <param name="referenceId">Reference ID</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the payment status and redirect URL</returns>
    Task<(bool Success, string Status, string Message, string RedirectUrl)> CheckPaymentStatusAsync(string referenceId);

    /// <summary>
    /// Links a transaction to an order after the order is created
    /// </summary>
    /// <param name="referenceId">Reference ID</param>
    /// <param name="orderId">Order ID</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the result</returns>
    Task<(bool Success, string Message)> LinkTransactionToOrderAsync(string referenceId, int orderId);

    /// <summary>
    /// Processes a callback
    /// </summary>
    /// <param name="model">Callback model</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the result</returns>
    Task<(bool Success, string Message)> ProcessCallbackAsync(MomoCallbackModel model);
}

