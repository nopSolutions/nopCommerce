using Nop.Plugin.Payments.Paystack.Models;

namespace Nop.Plugin.Payments.Paystack.Services;

/// <summary>
/// Paystack transaction service interface
/// </summary>
public interface IPaystackTransactionService
{
    /// <summary>
    /// Creates a new transaction record
    /// </summary>
    Task<PaystackTransactionModel> InsertTransactionAsync(string reference, string customerEmail, decimal amount, string currency, int orderId);

    /// <summary>
    /// Updates a transaction status
    /// </summary>
    Task<PaystackTransactionModel?> UpdateTransactionStatusAsync(string reference, string status, string? errorMessage = null);

    /// <summary>
    /// Gets a transaction by reference
    /// </summary>
    Task<PaystackTransactionModel?> GetTransactionAsync(string reference);

    /// <summary>
    /// Gets a transaction by order ID
    /// </summary>
    Task<PaystackTransactionModel?> GetTransactionByOrderIdAsync(int orderId);

    /// <summary>
    /// Gets transaction history
    /// </summary>
    Task<IList<PaystackTransactionModel>> GetTransactionHistoryAsync(DateTime? fromDate = null, DateTime? toDate = null, int? orderId = null);

    /// <summary>
    /// Validates webhook signature (X-Paystack-Signature) using plugin settings
    /// </summary>
    bool ValidateWebhookSignature(string payload, string signature);

    /// <summary>
    /// Validates webhook signature using the provided secret (e.g. for a specific store)
    /// </summary>
    bool ValidateWebhookSignature(string payload, string signature, string secret);
}
