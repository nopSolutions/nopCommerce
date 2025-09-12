using System.Net.Http.Headers;
using Newtonsoft.Json;
using Nop.Core.Domain.Tax;
using Nop.Core.Http;

namespace Nop.Services.Tax;

/// <summary>
/// Check vat service 
/// </summary>
public partial class CheckVatService : ICheckVatService
{
    #region Fields

    protected readonly IHttpClientFactory _httpClientFactory;
    protected readonly TaxSettings _taxSettings;

    #endregion

    #region Ctor

    public CheckVatService(IHttpClientFactory httpClientFactory, TaxSettings taxSettings)
    {
        _httpClientFactory = httpClientFactory;
        _taxSettings = taxSettings;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Gets hmrc.gov.uk API access token
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the access token for hmrc service
    /// </returns>
    protected virtual async Task<string> GetHmrcAccessTokenAsync()
    {
        if (string.IsNullOrEmpty(_taxSettings.HmrcClientId) || string.IsNullOrEmpty(_taxSettings.HmrcClientSecret))
            return null;

        var client = _httpClientFactory.CreateClient(NopHttpDefaults.DefaultHttpClient);
        client.BaseAddress = new Uri(_taxSettings.HmrcApiUrl);

        var data = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("client_id", _taxSettings.HmrcClientId),
            new KeyValuePair<string, string>("client_secret", _taxSettings.HmrcClientSecret),
            new KeyValuePair<string, string>("grant_type", "client_credentials"),
            new KeyValuePair<string, string>("scope", "read:vat")
        });

        var response = await client.PostAsync(NopTaxDefaults.HmrcOauthTokenUrl, data);

        if (!response.IsSuccessStatusCode)
            return null;

        return JsonConvert.DeserializeAnonymousType(await response.Content.ReadAsStringAsync(),
            new
            {
                access_token = string.Empty,
                token_type = string.Empty,
                expires_in = 0
            })?.access_token;
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
    public virtual async Task<(VatNumberStatus vatNumberStatus, string name, string address)> CheckVatAsync(string twoLetterIsoCode, string vatNumber)
    {
        var name = string.Empty;
        var address = string.Empty;

        //as of 01/01/2021, the VoW service to validate UK (GB) VAT numbers ceased to exist.
        //so we use hmrc.gov.uk API to validate UK VAT numbers.
        if (twoLetterIsoCode == "GB")
        {
            var accessToken = await GetHmrcAccessTokenAsync();

            if (string.IsNullOrEmpty(accessToken))
                return (VatNumberStatus.Invalid, name, address);

            var client = _httpClientFactory.CreateClient(NopHttpDefaults.DefaultHttpClient);
            client.BaseAddress = new Uri(_taxSettings.HmrcApiUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.hmrc.2.0+json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await client.GetAsync(string.Format(NopTaxDefaults.HmrcVatValidateUrl, vatNumber));

            if (!response.IsSuccessStatusCode)
                return (VatNumberStatus.Invalid, name, address);

            var result = JsonConvert.DeserializeAnonymousType(await response.Content.ReadAsStringAsync(),
                new { target = new { name = string.Empty, address = new { line1 = string.Empty } } });

            if (result == null)
                return (VatNumberStatus.Invalid, name, address);
           
            name = result.target.name;
            address = result.target.address.line1;
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