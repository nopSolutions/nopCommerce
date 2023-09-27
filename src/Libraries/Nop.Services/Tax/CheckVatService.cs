using Newtonsoft.Json;
using Nop.Core.Domain.Tax;
using Nop.Core.Http;

namespace Nop.Services.Tax
{
    /// <summary>
    /// Check vat service 
    /// </summary>
    public partial class CheckVatService : ICheckVatService
    {
        #region Fields

        protected readonly IHttpClientFactory _httpClientFactory;

        #endregion

        #region Ctor

        public CheckVatService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        #endregion

        #region Methods

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
            var name = string.Empty;
            var address = string.Empty;

            //as of 01/01/2021, the VoW service to validate UK (GB) VAT numbers ceased to exist.
            //so we use hmrc.gov.uk API to validate UK VAT numbers.
            if (twoLetterIsoCode == "GB")
            {
                var client = _httpClientFactory.CreateClient(NopHttpDefaults.DefaultHttpClient);
                var result = await client.GetAsync(string.Format(NopTaxDefaults.UKVatValidateUrl, vatNumber));

                if (!result.IsSuccessStatusCode)
                    return (VatNumberStatus.Invalid, name, address);

                var response = JsonConvert.DeserializeAnonymousType(await result.Content.ReadAsStringAsync(),
                    new { target = new { name = string.Empty, address = new { line1 = string.Empty } } });

                if (response == null) 
                    return (VatNumberStatus.Invalid, name, address);

                var target = response.target;
                name = target.name;
                address = target.address.line1;
            }
            else
            {
                //the service returns INVALID_INPUT for country codes that are not uppercase.
                twoLetterIsoCode = twoLetterIsoCode.ToUpperInvariant();

                var s = new EuropaCheckVatService.checkVatPortTypeClient();
                var result = await s.checkVatAsync(new EuropaCheckVatService.checkVatRequest
                {
                    vatNumber = vatNumber,
                    countryCode = twoLetterIsoCode
                });

                if (!result.valid)
                    return (VatNumberStatus.Invalid, name, address);

                name = result.name;
                address = result.address;
            }

            return (VatNumberStatus.Valid, name, address);
        }

        #endregion
    }
}
