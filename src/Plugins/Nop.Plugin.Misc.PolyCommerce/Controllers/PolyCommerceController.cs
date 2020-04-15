using System;
using System.Data;
using System.Data.SqlClient;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Directory;
using Nop.Data;
using Nop.Plugin.Misc.PolyCommerce.Models;
using Nop.Services.Directory;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Services.Logging;

namespace Nop.Plugin.Misc.PolyCommerce.Controllers
{
    public class PolyCommerceController : BasePluginController
    {
        private readonly IDbContext _dbContext;
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;
        private readonly CurrencySettings _currencySettings;
        private readonly ICurrencyService _currencyService;
        private readonly ILogger _logger;
        private const string POLY_COMMERCE_BASE_URL = "https://localhost:44340";
        private const int NOP_COMMERCE = 2;

        public PolyCommerceController(IDbContext dbContext,
            IStoreContext storeContext,
            IWorkContext workContext,
            ICurrencyService currencyService,
            CurrencySettings currencySettings,
            ILogger logger)
        {
            _dbContext = dbContext;
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

                string storeToken = null;

                using (var conn = new SqlConnection(dataSettings.DataConnectionString))
                {
                    using (var command = new SqlCommand())
                    {
                        command.CommandText = @"select Token
                                            from[dbo].[PolyCommerceStore]
                                            where StoreId = @StoreId";

                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@StoreId", _storeContext.CurrentStore.Id));

                        command.Connection = conn;

                        await conn.OpenAsync();

                        storeToken = (await command.ExecuteScalarAsync())?.ToString();
                    }
                }

                if (storeToken == null)
                {

                    storeToken = Guid.NewGuid().ToString("N").ToLower();
                    var affectedRecords = _dbContext.ExecuteSqlCommand("insert into [dbo].[PolyCommerceStore](StoreId, Token, CreatedOnUtc, IsActive) values(@StoreId, @Token, @CreatedOnUtc, @IsActive)", false, null,
                        new SqlParameter("@StoreId", _storeContext.CurrentStore.Id),
                        new SqlParameter("@Token", storeToken),
                        new SqlParameter("@CreatedOnUtc", DateTime.UtcNow),
                        new SqlParameter("@IsActive", true));

                    if (affectedRecords != 1)
                    {
                        // return error view
                        _logger.Error($"PolyCommerce | There was a problem inserting into table [dbo].[PolyCommerceStore]. Expected 1 record to be inserted but instead {affectedRecords} records were affected.");
                        return Redirect($"{POLY_COMMERCE_BASE_URL}/unexpected-error");
                    }
                }

                var primaryStoreCurrency = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId).CurrencyCode;



                var request = new LoginModel
                {
                    StoreName = _storeContext.CurrentStore.Name,
                    CustomerGuid = _workContext.CurrentCustomer.CustomerGuid.ToString(),
                    UserId = _workContext.CurrentCustomer.Id.ToString(),
                    StoreUrl = _storeContext.CurrentStore.Url,
                    StoreToken = storeToken,
                    Username = _workContext.CurrentCustomer.Username,
                    UserEmail = _workContext.CurrentCustomer.Email,
                    StoreIntegrationTypeId = NOP_COMMERCE,
                    StoreCurrencyCode = primaryStoreCurrency
                };

                string userToken = null;

                using (var client = new HttpClient())
                {
                    // get short lived token for user..
                    var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                    var result = await client.PostAsync($"{POLY_COMMERCE_BASE_URL}/api/account/generate_external_token", content);

                    var resultString = await result.Content.ReadAsStringAsync();

                    if (result.IsSuccessStatusCode)
                    {
                        _logger.Information($"PolyCommerce | Successfully generated authentication token for user '{_workContext.CurrentCustomer.Username}'.");
                        userToken = resultString;
                    }
                    else
                    {
                        // return error view
                        _logger.Error($"PolyCommerce | Could not generate authentication token. {userToken}");
                        return Redirect($"{POLY_COMMERCE_BASE_URL}/unexpected-error");
                    }
                }

                return Redirect($"{POLY_COMMERCE_BASE_URL}/account/login?token={userToken}");
            }
            catch (Exception ex)
            {
                _logger.Error("PolyCommerce | Could not generate authentication token.", ex);
                return Redirect($"{POLY_COMMERCE_BASE_URL}/unexpected-error");
            }
        }
    }
}
