using Nop.Plugin.Payments.Paystack.Models;

namespace Nop.Plugin.Payments.Paystack.Helpers;

public static class PaystackModelHelpers
{
    public static PaystackTransactionModel CreateTransactionModel(string reference, string customerEmail, decimal amount, string currency, int orderId)
    {
        return new PaystackTransactionModel
        {
            Reference = reference,
            CustomerEmail = customerEmail,
            Amount = amount,
            Currency = currency,
            CreatedAt = DateTime.Now,
            Status = "pending",
            OrderId = orderId,
        };
    }
}