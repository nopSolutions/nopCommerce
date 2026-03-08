using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Plugin.Payments.Momo.Models;

namespace Nop.Plugin.Payments.Momo.Services;

/// <summary>
/// MoMo transaction service interface
/// </summary>
public interface IMomoTransactionService
{
    /// <summary>
    /// Creates a new transaction
    /// </summary>
    Task<MomoTransaction> CreateTransactionAsync(string phoneNumber, decimal amount, string currency, int orderId, string referenceId);

    /// <summary>
    /// Updates a transaction status
    /// </summary>
    Task<MomoTransaction> UpdateTransactionStatusAsync(string referenceId, string status, string errorMessage = null);

    /// <summary>
    /// Updates a transaction with order ID
    /// </summary>
    Task<MomoTransaction> UpdateTransactionOrderIdAsync(string referenceId, int orderId);

    /// <summary>
    /// Gets a transaction by reference ID
    /// </summary>
    Task<MomoTransaction> GetTransactionAsync(string referenceId);

    /// <summary>
    /// Gets transaction history
    /// </summary>
    Task<IList<MomoTransaction>> GetTransactionHistoryAsync(DateTime? fromDate = null, DateTime? toDate = null, int? orderId = null);

    /// <summary>
    /// Validates callback signature
    /// </summary>
    Task<bool> ValidateCallbackSignatureAsync(MomoCallbackModel callback);
}
