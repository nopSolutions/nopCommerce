using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Expo.Server.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Gdpr;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Tax;
using Nop.Core.Events;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.ExportImport;
using Nop.Services.Forums;
using Nop.Services.Gdpr;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Customers;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Models.Api.Security;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class PushNotificationController : BaseAdminController
    {
        #region Fields

        private readonly ICustomerService _customerService;
        private readonly IPermissionService _permissionService;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;

        #endregion

        #region Ctor

        public PushNotificationController(
            ICustomerService customerService,
            IPermissionService permissionService,
            ILocalizationService localizationService,
            INotificationService notificationService)
        {
            _customerService = customerService;
            _permissionService = permissionService;
            _localizationService = localizationService;
            _notificationService = notificationService;
        }

        #endregion

        #region Push Notifcation

        public virtual async Task<IActionResult> SendNotification()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            //prepare model
            var model = new PushNotificationModel();

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> SendNotification(PushNotificationModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            try
            {
                IPagedList<Customer> notificationCustomers = await _customerService.GetAllPushNotificationCustomersAsync(sendToAll: true);
                foreach (var customer in notificationCustomers)
                {
                    if (!string.IsNullOrEmpty(customer.PushToken))
                    {
                        var expoSDKClient = new PushApiClient();
                        var pushTicketReq = new PushTicketRequest()
                        {
                            PushTo = new List<string>() { customer.PushToken },
                            PushTitle = model.MessageTitle,
                            PushBody = model.MessageBody
                        };
                        var result = await expoSDKClient.PushSendAsync(pushTicketReq);
                    }
                }
            }
            catch (Exception ex)
            {
                _notificationService.SuccessNotification(ex.Message);
            }
            //prepare model
            model = new PushNotificationModel();

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.PushNotification.Send.Successfully"));
            //if we got this far, something failed, redisplay form
            return View(model);
        }

        #endregion
    }
}