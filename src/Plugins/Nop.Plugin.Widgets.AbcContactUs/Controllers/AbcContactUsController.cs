using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Security;
using Nop.Data;
using Nop.Plugin.Misc.AbcCore.Domain;
using Nop.Plugin.Widgets.AbcContactUs;
using Nop.Services.Customers;
using Nop.Services.Messages;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using SevenSpikes.Nop.Plugins.StoreLocator.Domain.Shops;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Nop.Web.Framework.Security.Captcha;
using System;
using Nop.Services.Logging;
using Nop.Plugin.Widgets.AbcContactUs.Models;
using Nop.Services.Localization;
using Nop.Services.Configuration;
using Nop.Web.Framework;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.AbcHomeDeliveryStatus.Controllers
{
    public class AbcContactUsController : BasePluginController
    {
        private readonly IWorkContext _workContext;
        private readonly IRepository<Shop> _shopRepository;
        private readonly IRepository<ShopAbc> _shopAbcRepository;
        private readonly EmailAccountSettings _emailAccountSettings;
        private readonly IEmailAccountService _emailAccountService;
        private readonly IEmailSender _emailSender;
        private readonly ContactUsWidgetSettings _contactUsWidgetSettings;
        private readonly CaptchaSettings _captchaSettings;
        private readonly ContactUsWidgetSettings _settings;
        private readonly ICustomerService _customerService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly CaptchaHttpClient _captchaHttpClient;
        private readonly ILogger _logger;
        private readonly INotificationService _notificationService;
        private readonly ILocalizationService _localizationService;
        private readonly ISettingService _settingService;

        public AbcContactUsController(
            IWorkContext workContext,
            IRepository<Shop> shopRepository,
            IRepository<ShopAbc> shopAbcRepository,
            EmailAccountSettings emailAccountSettings,
            IEmailAccountService emailAccountService,
            IEmailSender emailSender,
            ContactUsWidgetSettings contactUsWidgetSettings,
            CaptchaSettings captchaSettings,
            ContactUsWidgetSettings settings,
            ICustomerService customerService,
            IHttpContextAccessor httpContextAccessor,
            CaptchaHttpClient captchaHttpClient,
            ILogger logger,
            INotificationService notificationService,
            ILocalizationService localizationService,
            ISettingService settingService
        )
        {
            _workContext = workContext;
            _shopRepository = shopRepository;
            _shopAbcRepository = shopAbcRepository;
            _emailAccountSettings = emailAccountSettings;
            _emailAccountService = emailAccountService;
            _emailSender = emailSender;
            _contactUsWidgetSettings = contactUsWidgetSettings;
            _captchaSettings = captchaSettings;
            _settings = settings;
            _customerService = customerService;
            _httpContextAccessor = httpContextAccessor;
            _captchaHttpClient = captchaHttpClient;
            _logger = logger;
            _notificationService = notificationService;
            _localizationService = localizationService;
            _settingService = settingService;
        }

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public IActionResult Configure()
        {
            return View(
                "~/Plugins/Widgets.AbcContactUs/Views/Configure.cshtml",
                _contactUsWidgetSettings.ToModel()
            );
        }

        [HttpPost]
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Configure(ContactUsConfigModel model)
        {
            await _settingService.SaveSettingAsync(ContactUsWidgetSettings.FromModel(model));
            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

            return Configure();
        }

        [HttpPost]
        public async Task<IActionResult> DisplayContactUs(ContactUsModel model)
        {
            var resultModel = new ContactUsResultModel();

            if (_captchaSettings.Enabled)
            {
                var gCaptchaResponseValue = model.GRecaptchaResponse;

                if (StringValues.IsNullOrEmpty(gCaptchaResponseValue))
                {
                    return BadRequest();
                }

                var response = _captchaHttpClient.ValidateCaptchaAsync(gCaptchaResponseValue).Result;
                if (!response.IsValid)
                {
                    return BadRequest();
                }
            }

            var account = await _emailAccountService.GetEmailAccountByIdAsync(_emailAccountSettings.DefaultEmailAccountId);
            var ccEmails = new List<string>(_contactUsWidgetSettings.AdditionalEmails.Split(','));

            var toAddress = "";
            if (model.SelectedStore == "Website")
            {
                toAddress = _settings.ContactUsEmail ?? "support@abcwarehouse.com";
            }
            else
            {
                var shop = _shopRepository.Table.Where(s => s.Name == model.SelectedStore).FirstOrDefault();
                toAddress = _shopAbcRepository.Table.Where(sabc => sabc.ShopId == shop.Id).FirstOrDefault().AbcEmail;
                ccEmails.Add(_settings.ContactUsEmail);
            }

            var subject = "Customer " + model.Reason + "- " + model.SelectedStore + "/" + model.Name + "  " + model.PhoneNumber;

            var body = string.Format(
                @"A request has been submitted with an inquiry from the Contact us page. <br/><br/>

                Customer Name: {0} <br/>
                Customer Email: {1} <br/>
                Customer Phone Number: {2} <br/>
                Store Location: {3} <br/>
                Comments: {4}", model.Name ?? "", model.Email ?? "", model.PhoneNumber ?? "", model.SelectedStore ?? "", model.Comments ?? "");

            if (_settings.IsEmailSubmissionSkipped)
            {
                await _logger.WarningAsync($"AbcContactUs: Email submission skipped - body: {body}");
            }
            else
            {
                await _emailSender.SendEmailAsync(account, subject, body, account.Email, account.DisplayName, toAddress, "", replyToAddress: model.Email, cc: ccEmails);
            }

            return Content("");
        }
    }
}


