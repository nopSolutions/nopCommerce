using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Data;
using Nop.Plugin.Payments.Momo.Models;

namespace Nop.Plugin.Payments.Momo.Services;

/// <summary>
/// MoMo transaction service
/// </summary>
public class MomoTransactionService : IMomoTransactionService
{
    #region Fields

    private readonly IRepository<MomoTransactionModel> _transactionRepository;
    private readonly MomoPaymentSettings _momoPaymentSettings;

    #endregion

    #region Ctor

    public MomoTransactionService(
        IRepository<MomoTransactionModel> transactionRepository,
        MomoPaymentSettings momoPaymentSettings)
    {
        _transactionRepository = transactionRepository;
        _momoPaymentSettings = momoPaymentSettings;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Creates a new transaction
    /// </summary>
    public virtual async Task<MomoTransactionModel> CreateTransactionAsync(string phoneNumber, decimal amount, string currency, int orderId)
    {
        var transaction = new MomoTransactionModel
        {
            ReferenceId = Guid.NewGuid().ToString(),
            PhoneNumber = phoneNumber,
            Amount = amount,
            Currency = currency,
            CreatedAt = DateTime.UtcNow,
            Status = "PENDING",
            OrderId = orderId
        };

        await _transactionRepository.InsertAsync(transaction);

        return transaction;
    }

    /// <summary>
    /// Updates a transaction status
    /// </summary>
    public virtual async Task<MomoTransactionModel> UpdateTransactionStatusAsync(string referenceId, string status, string errorMessage = null)
    {
        var transaction = await _transactionRepository.Table
            .FirstOrDefaultAsync(t => t.ReferenceId == referenceId);

        if (transaction == null)
            return null;

        transaction.Status = status;
        transaction.ErrorMessage = errorMessage;
        transaction.UpdatedAt = DateTime.UtcNow;

        await _transactionRepository.UpdateAsync(transaction);

        return transaction;
    }

    /// <summary>
    /// Gets a transaction by reference ID
    /// </summary>
    public virtual async Task<MomoTransactionModel> GetTransactionAsync(string referenceId)
    {
        return await _transactionRepository.Table
            .FirstOrDefaultAsync(t => t.ReferenceId == referenceId);
    }

    /// <summary>
    /// Gets transaction history
    /// </summary>
    public virtual async Task<IList<MomoTransactionModel>> GetTransactionHistoryAsync(DateTime? fromDate = null, DateTime? toDate = null, int? orderId = null)
    {
        var query = _transactionRepository.Table;

        if (fromDate.HasValue)
            query = query.Where(t => t.CreatedAt >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(t => t.CreatedAt <= toDate.Value);

        if (orderId.HasValue)
            query = query.Where(t => t.OrderId == orderId.Value);

        return await query.OrderByDescending(t => t.CreatedAt).ToListAsync();
    }

    /// <summary>
    /// Validates callback signature
    /// </summary>
    public virtual async Task<bool> ValidateCallbackSignatureAsync(MomoCallbackModel callback)
    {
        if (!_momoPaymentSettings.EnableCallbackValidation)
            return true;

        // Implement signature validation logic here
        // This will depend on MTN MoMo's signature requirements
        return true;
    }

    #endregion
}
