using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Orders;
using Nop.Data;
using Nop.Plugin.Misc.PolyCommerce.Models;
using Nop.Services.Directory;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Misc.PolyCommerce.Controllers
{
    public class PolyCommerceController : BasePluginController
    {
        private readonly IDbContext _dbContext;
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;
        private readonly CurrencySettings _currencySettings;
        private readonly ICurrencyService _currencyService;
        private const string POLY_COMMERCE_BASE_URL = "https://localhost:44340";

        public PolyCommerceController(IDbContext dbContext, IStoreContext storeContext, IWorkContext workContext, ICurrencyService currencyService, CurrencySettings currencySettings)
        {
            _dbContext = dbContext;
            _currencyService = currencyService;
            _storeContext = storeContext;
            _workContext = workContext;
            _currencySettings = currencySettings;
        }

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        [HttpGet]
        public async Task<IActionResult> Configure()
        {
            // get short lived token for user..
            
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
                StoreIntegrationTypeId = 2,
                StoreCurrencyCode = primaryStoreCurrency
            };

            var model = new ConfigureViewModel()
            {
                PolyCommerceBaseUrl = POLY_COMMERCE_BASE_URL
            };

            using (var client = new HttpClient())
            {
                var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                var result = await client.PostAsync($"{POLY_COMMERCE_BASE_URL}/api/account/generate_external_token", content);

                var resultString = await result.Content.ReadAsStringAsync();

                if (result.IsSuccessStatusCode)
                {
                    model.UserToken = resultString;
                }
                else
                {
                    // return error view
                }

                
            }

            return View("~/Plugins/Misc.PolyCommerce/Views/Configure.cshtml", model);
        }
    }
}
