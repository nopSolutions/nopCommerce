using Nop.Data;
using Nop.Plugin.Payments.Momo.Models;

namespace Nop.Plugin.Payments.Momo.Services;

/// <summary>
/// MoMo transaction service
/// </summary>
public class MomoTransactionService : IMomoTransactionService
{
    #region Fields

    private readonly IRepository<MomoTransaction> _transactionRepository;
    private readonly MomoPaymentSettings _momoPaymentSettings;

    #endregion

    #region Ctor

    public MomoTransactionService(
        IRepository<MomoTransaction> transactionRepository,
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
    public virtual async Task<MomoTransaction> CreateTransactionAsync(string phoneNumber, decimal amount, string currency, int orderId, string referenceId)
    {
        var transaction = new MomoTransaction
        {
            ReferenceId = referenceId,
            PhoneNumber = phoneNumber,
            Amount = amount,
            Currency = currency,
            CreatedAt = DateTime.Now,
            Status = "PENDING",
            OrderId = orderId
        };

        await _transactionRepository.InsertAsync(transaction);

        return transaction;
    }

    /// <summary>
    /// Updates a transaction status
    /// </summary>
    public virtual async Task<MomoTransaction> UpdateTransactionStatusAsync(string referenceId, string status, string errorMessage = null)
    {
        var transaction = await _transactionRepository.Table
            .FirstOrDefaultAsync(t => t.ReferenceId == referenceId);

        if (transaction == null)
            return null;

        transaction.Status = status;
        transaction.ErrorMessage = errorMessage;
        transaction.UpdatedAt = DateTime.Now;

        await _transactionRepository.UpdateAsync(transaction);

        return transaction;
    }

    /// <summary>
    /// Updates a transaction with order ID
    /// </summary>
    public virtual async Task<MomoTransaction> UpdateTransactionOrderIdAsync(string referenceId, int orderId)
    {
        var transaction = await _transactionRepository.Table
            .FirstOrDefaultAsync(t => t.ReferenceId == referenceId);

        if (transaction == null)
            return null;

        transaction.OrderId = orderId;
        transaction.UpdatedAt = DateTime.Now;

        await _transactionRepository.UpdateAsync(transaction);

        return transaction;
    }

    /// <summary>
    /// Gets a transaction by reference ID
    /// </summary>
    public virtual async Task<MomoTransaction> GetTransactionAsync(string referenceId)
    {
        return await _transactionRepository.Table
            .FirstOrDefaultAsync(t => t.ReferenceId == referenceId);
    }

    /// <summary>
    /// Gets transaction history
    /// </summary>
    public virtual async Task<IList<MomoTransaction>> GetTransactionHistoryAsync(DateTime? fromDate = null, DateTime? toDate = null, int? orderId = null)
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
