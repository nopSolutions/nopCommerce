namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models.Enums;

/// <summary>
/// Represents the type of the vault instruction
/// </summary>
public enum VaultInstructionType
{
    /// <summary>
    /// Defines that the payment_source will be vaulted only when at least one authorization or capture using that payment_source is successful.
    /// </summary>
    ON_SUCCESS
}