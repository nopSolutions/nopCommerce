using System;
using System.Linq;
using System.Web.Mvc;
using Nop.Admin.Models.Customers;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Security;
using Nop.Web.Framework.Controllers;
using Telerik.Web.Mvc;

namespace Nop.Admin.Controllers
{
    [AdminAuthorize]
    public class OnlineCustomerController : BaseNopController
    {
        #region Fields

        private readonly ICustomerService _customerService;
        private readonly IGeoCountryLookup _geoCountryLookup;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly CustomerSettings _customerSettings;
        private readonly AdminAreaSettings _adminAreaSettings;
        private readonly IPermissionService _permissionService;

        #endregion

        #region Constructors

        public OnlineCustomerController(ICustomerService customerService,
            IGeoCountryLookup geoCountryLookup, IDateTimeHelper dateTimeHelper,
            CustomerSettings customerSettings, AdminAreaSettings adminAreaSettings,
            IPermissionService permissionService)
        {
            this._customerService = customerService;
            this._geoCountryLookup = geoCountryLookup;
            this._dateTimeHelper = dateTimeHelper;
            this._customerSettings = customerSettings;
            this._adminAreaSettings = adminAreaSettings;
            this._permissionService = permissionService;
        }

        #endregion
        
        #region Online customers

        public ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            var customers = _customerService.GetOnlineCustomers(DateTime.UtcNow.AddMinutes(-_customerSettings.OnlineCustomerMinutes),
                null, 0, _adminAreaSettings.GridPageSize);

            var model = new GridModel<OnlineCustomerModel>
            {
                Data = customers.Select(x =>
                {
                    return new OnlineCustomerModel()
                    {
                        Id = x.Id,
                        CustomerInfo = x.IsRegistered() ? x.Email : "Guest",
                        LastIpAddress = x.LastIpAddress,
                        Location = _geoCountryLookup.LookupCountryName(x.LastIpAddress),
                        LastActivityDate = _dateTimeHelper.ConvertToUserTime(x.LastActivityDateUtc, DateTimeKind.Utc),
                        LastVisitedPage = x.GetAttribute<string>(SystemCustomerAttributeNames.LastVisitedPage)
                    };
                }),
                Total = customers.TotalCount
            };
            return View(model);
        }

        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult List(GridCommand command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            var customers = _customerService.GetOnlineCustomers(DateTime.UtcNow.AddMinutes(-_customerSettings.OnlineCustomerMinutes),
                null, command.Page - 1, command.PageSize);
            var model = new GridModel<OnlineCustomerModel>
            {
                Data = customers.Select(x =>
                {
                    return new OnlineCustomerModel()
                    {
                        Id = x.Id,
                        CustomerInfo = x.IsRegistered() ? x.Email : "Guest",
                        LastIpAddress = x.LastIpAddress,
                        Location = _geoCountryLookup.LookupCountryName(x.LastIpAddress),
                        LastActivityDate = _dateTimeHelper.ConvertToUserTime(x.LastActivityDateUtc, DateTimeKind.Utc),
                        LastVisitedPage = x.GetAttribute<string>(SystemCustomerAttributeNames.LastVisitedPage)
                    };
                }),
                Total = customers.TotalCount
            };
            return new JsonResult
            {
                Data = model
            };
        }

        #endregion
    }
}
