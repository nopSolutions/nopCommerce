using Nop.Core.Domain.Tax;
using Nop.Services.Tax;

namespace Nop.Tests.Nop.Services.Tests.Tax;

public class TestCheckVatService: ICheckVatService
{
    private ICheckVatService _baseService;

    public TestCheckVatService(IHttpClientFactory httpClientFactory, TaxSettings taxSettings)
    {
        _baseService = new CheckVatService(httpClientFactory, taxSettings);
    }

    /// <summary>
    /// Try to validate VAT number
    /// </summary>
    /// <param name="twoLetterIsoCode">Two letter ISO code of a country</param>
    /// <param name="vatNumber">The VAT number to check</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the vAT Number status. Name (if received). Address (if received)
    /// </returns>
    public async Task<(VatNumberStatus vatNumberStatus, string name, string address)> CheckVatAsync(string twoLetterIsoCode, string vatNumber)
    {
        var testNumbers = new List<string> { "553557881", "974761076", "430479893", "00478390347" };

        if (testNumbers.Contains(vatNumber.ToUpper()))
            return await _baseService.CheckVatAsync(twoLetterIsoCode, vatNumber);

        return (VatNumberStatus.Valid, "Test Company", "Test Address");
    }
}
