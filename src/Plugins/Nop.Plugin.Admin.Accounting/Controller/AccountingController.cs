using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Admin.Accounting.Models;
using Nop.Services.Configuration;
using Nop.Services.Logging;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc.Filters;
using System;
using System.Threading.Tasks;

namespace Nop.Plugin.Admin.Accounting.Controller
{
    public class AccountingController : BaseAdminController
    {
        private readonly AccountingSettings _accountingSettings;
        private readonly ILogger _logger;
        private readonly ISettingService _settingService;

        public AccountingController(ILogger logger, AccountingSettings accountingSettings, ISettingService settingService)
        {
            _logger = logger;       
            _accountingSettings = accountingSettings;
            _settingService = settingService;
        }

        [AuthorizeAdmin]
        [Area(AreaNames.ADMIN)]
        public async Task<IActionResult> Configure()
        {
            AccountingConfigureModel model = null;

            try
            {
                model = GetBaseModel();
            }
            catch (Exception ex)
            {
                Exception inner = ex;
                while (inner.InnerException != null) inner = inner.InnerException;
                await _logger.ErrorAsync("Configure Accounting: " + inner.Message, ex);
                model.ErrorMessage += "<br />" + inner.Message;
            }

            return await Task.Run(() => View("~/Plugins/Admin.Accounting/Views/Configure.cshtml", model));
        }

        [HttpPost]
        [AuthorizeAdmin]
        [Area(AreaNames.ADMIN)]
        public async Task<IActionResult> ConfigureAsync(AccountingConfigureModel model)
        {
            if (!ModelState.IsValid)
                return View();

            try
            {
                _accountingSettings.ReconciliationListDaysBack = model.ReconciliationListDaysBack;
                _accountingSettings.PhysicalShop_ProductNumber = model.PhysicalShop_ProductNumber;
                _accountingSettings.WebshopDK_ProductNumber = model.WebshopDK_ProductNumber;
                _accountingSettings.WebshopSE_ProductNumber = model.WebshopSE_ProductNumber;
                _accountingSettings.WebshopOther_ProductNumber = model.WebshopOther_ProductNumber;
                _accountingSettings.WebshopOtherWithoutTax_ProductNumber = model.WebshopOtherWithoutTax_ProductNumber;

                await _settingService.SaveSettingAsync(_accountingSettings);
            }
            catch (Exception ex)
            {
                Exception inner = ex;
                while (inner.InnerException != null) inner = inner.InnerException;
                await _logger.ErrorAsync("Configure Accounting: " + inner.Message, ex);
                model.ErrorMessage += "<br />" + inner.Message;
            }

            return View("~/Plugins/Admin.Accounting/Views/Configure.cshtml", model);
        }

        private AccountingConfigureModel GetBaseModel()
        {
            return new AccountingConfigureModel
            {
                ReconciliationListDaysBack = _accountingSettings.ReconciliationListDaysBack,
                PhysicalShop_ProductNumber = _accountingSettings.PhysicalShop_ProductNumber,
                WebshopDK_ProductNumber = _accountingSettings?.WebshopDK_ProductNumber,
                WebshopSE_ProductNumber = _accountingSettings.WebshopSE_ProductNumber,
                WebshopOther_ProductNumber = _accountingSettings.WebshopOther_ProductNumber,
                WebshopOtherWithoutTax_ProductNumber = _accountingSettings.WebshopOtherWithoutTax_ProductNumber
            };
        }
    }
}
