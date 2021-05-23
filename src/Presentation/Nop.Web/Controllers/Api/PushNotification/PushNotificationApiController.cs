using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Vendors;
using Nop.Core.Events;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Events;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Services.Vendors;
using Nop.Web.Factories;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Models.Catalog;
using Nop.Web.Models.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Controllers.Api.Security
{
    [Produces("application/json")]
    [Route("api/push-notification")]
    [AuthorizeAttribute]
    public class PushNotificationApiController : BaseApiController
    {
        #region Fields

        private readonly IWorkContext _workContext;
        private readonly ICustomerService _customerService;
        private readonly ILocalizationService _localizationService;
        #endregion

        #region Ctor

        public PushNotificationApiController(
            IWorkContext workContext,
            ICustomerService customerService,
            ILocalizationService localizationService)
        {
            _workContext = workContext;
            _customerService = customerService;
            _localizationService = localizationService;
        }

        #endregion

        #region Nested Class

        public class PushNotifcationModel
        {
            public bool OrderStatusNotification { get; set; }
            public bool RemindMeNotification { get; set; }
            public bool RateReminderNotification { get; set; }
        }

        #endregion

        #region Push Notification

        [HttpPost("save-notification-settings")]
        public async Task<IActionResult> SavePushNotification([FromBody] PushNotifcationModel model)
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            if (customer == null)
                return Ok(new { success = false, message = await _localizationService.GetResourceAsync("Customer.Not.Found") });

            customer.OrderStatusNotification = model.OrderStatusNotification;
            customer.RateReminderNotification = model.RateReminderNotification;
            customer.RemindMeNotification= model.RemindMeNotification;
            await _customerService.UpdateCustomerAsync(customer);

            return Ok(new { success = true, message = await _localizationService.GetResourceAsync("Customer.Notification.Settings.Updated") });
        }

        #endregion
    }
}
