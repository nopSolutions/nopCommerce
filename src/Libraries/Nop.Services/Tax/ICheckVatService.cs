using Nop.Core.Domain.Tax;

namespace Nop.Services.Tax;

/// <summary>
/// Check vat service interface
/// </summary>
public partial interface ICheckVatService
{
    /// <summary>
    /// Try to validate VAT number
    /// </summary>
    /// <param name="twoLetterIsoCode">Two letter ISO code of a country</param>
    /// <param name="vatNumber">The VAT number to check</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the vAT Number status. Name (if received). Address (if received)
    /// </returns>
    Task<(VatNumberStatus vatNumberStatus, string name, string address)> CheckVatAsync(string twoLetterIsoCode, string vatNumber);
}