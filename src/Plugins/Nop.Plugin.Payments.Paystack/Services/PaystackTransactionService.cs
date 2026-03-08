using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Nop.Data;
using Nop.Plugin.Payments.Paystack.Models;

namespace Nop.Plugin.Payments.Paystack.Services;

/// <summary>
/// Paystack transaction service
/// </summary>
public class PaystackTransactionService : IPaystackTransactionService
{
    private readonly IRepository<PaystackTransactionModel> _transactionRepository;
    private readonly PaystackPaymentSettings _settings;

    public PaystackTransactionService(
        IRepository<PaystackTransactionModel> transactionRepository,
        PaystackPaymentSettings settings)
    {
        _transactionRepository = transactionRepository;
        _settings = settings;
    }

    /// <inheritdoc />
    public virtual async Task<PaystackTransactionModel> CreateTransactionAsync(string reference, string customerEmail, decimal amount, string currency, int orderId)
    {
        var transaction = new PaystackTransactionModel
        {
            Reference = reference,
            CustomerEmail = customerEmail,
            Amount = amount,
            Currency = currency,
            CreatedAt = DateTime.Now,
            Status = "pending",
            OrderId = orderId
        };

        await _transactionRepository.InsertAsync(transaction);
        return transaction;
    }

    /// <inheritdoc />
    public virtual async Task<PaystackTransactionModel?> UpdateTransactionStatusAsync(string reference, string status, string? errorMessage = null)
    {
        var transaction = await _transactionRepository.Table
            .FirstOrDefaultAsync(t => t.Reference == reference);

        if (transaction == null)
            return null;

        transaction.Status = status;
        transaction.ErrorMessage = errorMessage;
        transaction.UpdatedAt = DateTime.Now;

        await _transactionRepository.UpdateAsync(transaction);
        return transaction;
    }

    /// <inheritdoc />
    public virtual async Task<PaystackTransactionModel?> GetTransactionAsync(string reference)
    {
        return await _transactionRepository.Table
            .FirstOrDefaultAsync(t => t.Reference == reference);
    }

    /// <inheritdoc />
    public virtual async Task<PaystackTransactionModel?> GetTransactionByOrderIdAsync(int orderId)
    {
        return await _transactionRepository.Table
            .FirstOrDefaultAsync(t => t.OrderId == orderId);
    }

    /// <inheritdoc />
    public virtual async Task<IList<PaystackTransactionModel>> GetTransactionHistoryAsync(DateTime? fromDate = null, DateTime? toDate = null, int? orderId = null)
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

    /// <inheritdoc />
    public virtual bool ValidateWebhookSignature(string payload, string signature)
    {
        var secret = !string.IsNullOrEmpty(_settings.WebhookSecret) ? _settings.WebhookSecret : _settings.SecretKey;
        return ValidateWebhookSignature(payload, signature, secret);
    }

    /// <inheritdoc />
    public virtual bool ValidateWebhookSignature(string payload, string signature, string secret)
    {
        if (string.IsNullOrEmpty(secret))
            return true;

        if (string.IsNullOrEmpty(signature))
            return false;

        using var hmacz = new HMACSHA512(Encoding.UTF8.GetBytes(secret));
        var hash = hmacz.ComputeHash(Encoding.UTF8.GetBytes(payload));
        var computed = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        return string.Equals(computed, signature, StringComparison.OrdinalIgnoreCase);
    }
}
