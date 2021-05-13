//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Rendering;
//using Nop.Core;
//using Nop.Core.Caching;
//using Nop.Core.Domain.Catalog;
//using Nop.Core.Domain.Localization;
//using Nop.Core.Domain.Media;
//using Nop.Core.Domain.Orders;
//using Nop.Core.Domain.Vendors;
//using Nop.Core.Events;
//using Nop.Services.Catalog;
//using Nop.Services.Common;
//using Nop.Services.Customers;
//using Nop.Services.Directory;
//using Nop.Services.Events;
//using Nop.Services.Helpers;
//using Nop.Services.Localization;
//using Nop.Services.Logging;
//using Nop.Services.Media;
//using Nop.Services.Messages;
//using Nop.Services.Orders;
//using Nop.Services.Payments;
//using Nop.Services.Security;
//using Nop.Services.Seo;
//using Nop.Services.Stores;
//using Nop.Services.Vendors;
//using Nop.Web.Factories;
//using Nop.Web.Framework.Mvc.Filters;
//using Nop.Web.Models.Catalog;
//using Nop.Web.Models.Common;
//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Linq;
//using System.Threading.Tasks;

//namespace Nop.Web.Controllers.Api.Security
//{
//    [Produces("application/json")]
//    [Route("api/push-notification")]
//    [AuthorizeAttribute]
//    public class PushNotificationApiController : BaseApiController
//    {
//        #region Fields

//        private readonly IWorkContext _workContext;
//        private readonly ICustomerService _customerService;

//        #endregion

//        #region Ctor

//        public PushNotificationApiController(
//            IWorkContext workContext,
//            ICustomerService customerService)
//        {
//            _workContext = workContext;
//            _customerService = customerService;
//        }

//        #endregion

//        #region Nested Class

//        public class PushNotifcationModel
//        {
//            public bool Offers { get; set; }
//            public bool Rewards { get; set; }
//            public bool EatsPass { get; set; }
//            public bool Other { get; set; }
//        }

//        #endregion

//        #region Push Notification

//        [HttpPost("save-notification-settings")]
//        public async Task<IActionResult> SavePushNotification(PushNotifcationModel model)
//        {
//            var customer = await _workContext.GetCurrentCustomerAsync();
//            if (customer == null)
//                return Ok(new { success = false, message = "'customer' could not be loaded" });

//            customer.Offers = model.Offers;
//            customer.Rewards = model.Rewards;
//            customer.EatsPass = model.EatsPass;
//            customer.Other = model.Other;
//            await _customerService.UpdateCustomerAsync(customer);

//            return Ok(new { success = true, message = "Notification settings updated successfully" });
//        }

//        #endregion
//    }
//}
