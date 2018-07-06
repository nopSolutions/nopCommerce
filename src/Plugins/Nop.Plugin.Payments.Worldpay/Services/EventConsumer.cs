using System.Linq;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Customers;
using Nop.Plugin.Payments.Worldpay.Models.Customer;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Payments;
using Nop.Web.Areas.Admin.Models.Customers;
using Nop.Web.Framework.Events;
using Nop.Web.Framework.Extensions;
using Nop.Web.Framework.UI;

namespace Nop.Plugin.Payments.Worldpay.Services
{
    /// <summary>
    /// Represents event consumer of the Worldpay payment plugin
    /// </summary>
    public class EventConsumer :
        IConsumer<PageRenderingEvent>,
        IConsumer<AdminTabStripCreated>
    {
        #region Fields

        private readonly ICustomerService _customerService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly IPaymentService _paymentService;
        private readonly WorldpayPaymentManager _worldpayPaymentManager;

        #endregion

        #region Ctor

        public EventConsumer(ICustomerService customerService,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            IPaymentService paymentService,
            WorldpayPaymentManager worldpayPaymentManager)
        {
            this._customerService = customerService;
            this._genericAttributeService = genericAttributeService;
            this._localizationService = localizationService;
            this._paymentService = paymentService;
            this._worldpayPaymentManager = worldpayPaymentManager;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handle page rendering event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        public void HandleEvent(PageRenderingEvent eventMessage)
        {
            if (eventMessage?.Helper?.ViewContext?.ActionDescriptor == null)
                return;

            //check whether the payment plugin is installed and is active
            var worldpayPaymentMethod = _paymentService.LoadPaymentMethodBySystemName(WorldpayPaymentDefaults.SystemName);
            if (!(worldpayPaymentMethod?.PluginDescriptor?.Installed ?? false) || !_paymentService.IsPaymentMethodActive(worldpayPaymentMethod))
                return;

            //add js sсript to one page checkout
            if (eventMessage.GetRouteNames().Any(r => r.Equals("CheckoutOnePage")))
                eventMessage.Helper.AddScriptParts(ResourceLocation.Footer, WorldpayPaymentDefaults.PaymentScriptPath, excludeFromBundle: true);
        }

        /// <summary>
        /// Handle admin tabstrip created event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        public async void HandleEvent(AdminTabStripCreated eventMessage)
        {
            if (eventMessage?.Helper == null)
                return;

            //we need customer details page
            var tabsElementId = "customer-edit";
            if (!eventMessage.TabStripName.Equals(tabsElementId))
                return;

            //check whether the payment plugin is installed and is active
            var worldpayPaymentMethod = _paymentService.LoadPaymentMethodBySystemName(WorldpayPaymentDefaults.SystemName);
            if (!(worldpayPaymentMethod?.PluginDescriptor?.Installed ?? false) || !_paymentService.IsPaymentMethodActive(worldpayPaymentMethod))
                return;

            //get the view model
            if (!(eventMessage.Helper.ViewData.Model is CustomerModel customerModel))
                return;

            //check whether a customer exists and isn't guest
            var customer = _customerService.GetCustomerById(customerModel.Id);
            if (customer == null || customer.IsGuest())
                return;

            //try to get stored in Vault customer
            var vaultCustomer = _worldpayPaymentManager.GetCustomer(_genericAttributeService.GetAttribute<string>(customer, WorldpayPaymentDefaults.CustomerIdAttribute));

            //prepare model
            var model = new WorldpayCustomerModel
            {
                Id = customerModel.Id,
                CustomerExists = vaultCustomer != null,
                WorldpayCustomerId = vaultCustomer?.CustomerId
            };

            //compose script to create a new tab
            var worldpayCustomerTabElementId = "tab-worldpay";
            var worldpayCustomerTab = new HtmlString($@"
                <script>
                    $(document).ready(function() {{
                        $(`
                            <li>
                                <a data-tab-name='{worldpayCustomerTabElementId}' data-toggle='tab' href='#{worldpayCustomerTabElementId}'>
                                    {_localizationService.GetResource("Plugins.Payments.Worldpay.WorldpayCustomer")}
                                </a>
                            </li>
                        `).appendTo('#{tabsElementId} .nav-tabs:first');
                        $(`
                            <div class='tab-pane' id='{worldpayCustomerTabElementId}'>
                                {
                                    (await eventMessage.Helper.PartialAsync("~/Plugins/Payments.Worldpay/Views/Customer/_CreateOrUpdate.Worldpay.cshtml", model)).RenderHtmlContent()
                                        .Replace("</script>", "<\\/script>") //we need escape a closing script tag to prevent terminating the script block early
                                }
                            </div>
                        `).appendTo('#{tabsElementId} .tab-content:first');
                    }});
                </script>");

            //add this tab as a block to render on the customer details page
            eventMessage.BlocksToRender.Add(worldpayCustomerTab);
        }

        #endregion
    }
}