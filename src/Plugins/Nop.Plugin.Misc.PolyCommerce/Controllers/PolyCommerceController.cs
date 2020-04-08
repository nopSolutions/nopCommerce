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
using Nop.Core.Domain.Common;
using Nop.Data;
using Nop.Plugin.Misc.PolyCommerce.Models;
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

        public PolyCommerceController(IDbContext dbContext, IStoreContext storeContext, IWorkContext workContext)
        {
            _dbContext = dbContext;
            _storeContext = storeContext;
            _workContext = workContext;
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
                    var parameter = new SqlParameter("@StoreId", _storeContext.CurrentStore.Id);
                    command.Parameters.Add(parameter);

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

            var request = new LoginModel
            {
                StoreName = _storeContext.CurrentStore.Name,
                CustomerGuid = _workContext.CurrentCustomer.CustomerGuid.ToString(),
                UserId = _workContext.CurrentCustomer.Id.ToString(),
                StoreUrl = _storeContext.CurrentStore.Url,
                StoreToken = storeToken,
                Username = _workContext.CurrentCustomer.Username,
                UserEmail = _workContext.CurrentCustomer.Email,
                StoreIntegrationTypeId = 2
            };

            var model = new ConfigureViewModel();

            using (var client = new HttpClient())
            {
                var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                var result = await client.PostAsync("https://localhost:44340/api/account/generate_external_token", content);

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
