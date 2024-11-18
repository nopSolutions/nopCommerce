using Nop.Core;
using Nop.Core.Domain.Messages;
using Nop.Plugin.Widgets.AbcContactUs.Models;
using Nop.Services.Logging;
using Nop.Services.Messages;
using SevenSpikes.Nop.Plugins.StoreLocator.Domain.Shops;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Data;
using Nop.Web.Framework.Components;
using Nop.Plugin.Misc.AbcCore.Domain;
using Nop.Core.Domain.Security;
using Nop.Services.Customers;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.AbcContactUs.Components
{
    [ViewComponent(Name = "AbcContactUs")]
    public class AbcContactUsViewComponent : NopViewComponent
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

        public AbcContactUsViewComponent(
            IWorkContext workContext,
            IRepository<Shop> shopRepository,
            IRepository<ShopAbc> shopAbcRepository,
            EmailAccountSettings emailAccountSettings,
            IEmailAccountService emailAccountService,
            IEmailSender emailSender,
            ContactUsWidgetSettings contactUsWidgetSettings,
            ILogger logger,
            CaptchaSettings captchaSettings,
            ContactUsWidgetSettings settings,
            ICustomerService customerService
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

        }

        public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData = null)
        {
            var topicSystemName = (string)additionalData;
            if (topicSystemName != "contact-us")
            {
                return Content("");
            }

            var model = new ContactUsModel();

            var customerAddress = (await _customerService.GetAddressesByCustomerIdAsync(
                (await _workContext.GetCurrentCustomerAsync()).Id
            )).FirstOrDefault();
            //prefilling based on customer address
            if (customerAddress != null)
            {
                model.Name = customerAddress.FirstName + ' ' + customerAddress.LastName;
                model.Email = customerAddress.Email;
                model.PhoneNumber = customerAddress.PhoneNumber;
            }

            PrepareContactReasonsAndStores(ref model);

            model.DisplayCaptcha = _captchaSettings.Enabled;

            return View("~/Plugins/Widgets.AbcContactUs/Views/ContactUs.cshtml", model);
        }

        private void PrepareContactReasonsAndStores(ref ContactUsModel model)
        {
            model.ReasonsForContact.Add(new SelectListItem { Text = "Comment", Value = "Comment" });
            model.ReasonsForContact.Add(new SelectListItem { Text = "Inquiry", Value = "Inquiry" });
            model.ReasonsForContact.Add(new SelectListItem { Text = "Solicitation", Value = "Solicitation" });
            model.ReasonsForContact.Add(new SelectListItem { Text = "Complaint", Value = "Complaint" });
            model.Reason = model.ReasonsForContact.First().Value;

            model.Stores.Add(new SelectListItem { Text = "Select Location", Value = "" });
            model.Stores.Add(new SelectListItem { Text = "Website", Value = "Website" });
            foreach (var shop in _shopRepository.Table)
            {
                model.Stores.Add(new SelectListItem { Text = shop.Name, Value = shop.Name });
            }

            model.SelectedStore = model.Stores.First().Value;

            model.DisplayCaptcha = _captchaSettings.Enabled;
        }
    }
}
