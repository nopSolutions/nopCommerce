using System;
using System.Data.SqlClient;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.Directory;
using Nop.Data;
using Nop.Plugin.Misc.PolyCommerce.Models;
using Nop.Services.Directory;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Services.Logging;
using Dapper;

namespace Nop.Plugin.Misc.PolyCommerce.Controllers
{
    public class PolyCommerceController : BasePluginController
    {
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;
        private readonly CurrencySettings _currencySettings;
        private readonly ICurrencyService _currencyService;
        private readonly ILogger _logger;
        private const int NOP_COMMERCE = 2;

        public PolyCommerceController(IStoreContext storeContext,
            IWorkContext workContext,
            ICurrencyService currencyService,
            CurrencySettings currencySettings,
            ILogger logger)
        {
            _currencyService = currencyService;
            _storeContext = storeContext;
            _workContext = workContext;
            _currencySettings = currencySettings;
            _logger = logger;
        }

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            try
            {
                var dataSettings = DataSettingsManager.LoadSettings();
                var currentStore = await _storeContext.GetCurrentStoreAsync();
                var currentCustomer = await _workContext.GetCurrentCustomerAsync();

                string storeToken = null;

                using (var conn = new SqlConnection(dataSettings.ConnectionString))
                {

                    storeToken = await conn.QueryFirstOrDefaultAsync<string>(@"select Token
                                                                    from[dbo].[PolyCommerceStore]
                                                                    where StoreId = @StoreId", new { StoreId = currentStore.Id });

                    if (storeToken == null)
                    {

                        storeToken = Guid.NewGuid().ToString("N").ToLower();

                        var affectedRecords = await conn.ExecuteAsync(@"insert into [dbo].[PolyCommerceStore]
                                                                        (
                                                                            StoreId, 
                                                                            Token, 
                                                                            CreatedOnUtc, 
                                                                            IsActive
                                                                        ) 
                                                                        values
                                                                        (
                                                                            @StoreId, 
                                                                            @Token,
                                                                            @CreatedOnUtc, 
                                                                            @IsActive
                                                                        )", new { StoreId = currentStore.Id, Token = storeToken, CreatedOnUtc = DateTime.UtcNow, IsActive = true });


                        if (affectedRecords < 1)
                        {
                            // return error view
                            await _logger.ErrorAsync($"PolyCommerce | There was a problem inserting into table [dbo].[PolyCommerceStore]. Expected 1 record to be inserted but instead {affectedRecords} records were affected.");
                            return Redirect($"{PolyCommerceHelper.GetBaseUrl()}/unexpected-error");
                        }
                    }
                }

                var primaryStoreCurrency = (await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId)).CurrencyCode;

                var storeUrl = new Uri($"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}");

                var request = new PolyCommerceLoginModel
                {
                    StoreName = currentStore.Name,
                    CustomerGuid = currentCustomer.CustomerGuid.ToString(),
                    UserId = currentCustomer.Id.ToString(),
                    StoreUrl = storeUrl.AbsoluteUri,
                    StoreToken = storeToken,
                    Username = currentCustomer.Username,
                    UserEmail = currentCustomer.Email,
                    StoreIntegrationTypeId = NOP_COMMERCE,
                    StoreCurrencyCode = primaryStoreCurrency
                };

                string userToken = null;

                var model = new ConfigureViewModel()
                {
                    PolyCommerceBaseUrl = PolyCommerceHelper.GetBaseUrl()
                };

                using (var client = new HttpClient())
                {
                    // get short lived token for user..
                    var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                    var result = await client.PostAsync($"{PolyCommerceHelper.GetBaseUrl()}/api/account/generate_external_token", content);

                    var resultString = await result.Content.ReadAsStringAsync();

                    if (result.IsSuccessStatusCode)
                    {
                        await _logger.InformationAsync($"PolyCommerce | Successfully generated authentication token for user '{currentCustomer.Username}'.");
                        userToken = resultString;
                    }
                    else
                    {
                        // return error view
                        await _logger.ErrorAsync($"PolyCommerce | Could not generate authentication token. {userToken}");
                        return Redirect($"{PolyCommerceHelper.GetBaseUrl()}/unexpected-error");
                    }
                }

                model.UserToken = userToken;

                return View("~/Plugins/Misc.PolyCommerce/Views/Dashboard.cshtml", model);
            }
            catch (Exception ex)
            {
                await _logger.ErrorAsync("PolyCommerce | Could not generate authentication token.", ex);
                return Redirect($"{PolyCommerceHelper.GetBaseUrl()}/unexpected-error");
            }
        }
    }
}
