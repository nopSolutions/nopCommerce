using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Customers;
using Nop.Services.Customers;
using Nop.Services.Security;
using Nop.Web.Factories;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework.Security;

namespace Nop.Web.Controllers
{
    [HttpsRequirement(SslRequirement.No)]
    public partial class ProfileController : BasePublicController
    {
        private readonly IProfileModelFactory _profileModelFactory;
        private readonly ICustomerService _customerService;
        private readonly IPermissionService _permissionService;
        private readonly CustomerSettings _customerSettings;

        public ProfileController(IProfileModelFactory profileModelFactory,
            ICustomerService customerService,
            IPermissionService permissionService,
            CustomerSettings customerSettings)
        {
            this._profileModelFactory = profileModelFactory;
            this._customerService = customerService;
            this._permissionService = permissionService;
            this._customerSettings = customerSettings;
        }

        public virtual IActionResult Index(int? id, int? pageNumber)
        {
            if (!_customerSettings.AllowViewingProfiles)
            {
                return RedirectToRoute("HomePage");
            }

            var customerId = 0;
            if (id.HasValue)
            {
                customerId = id.Value;
            }

            var customer = _customerService.GetCustomerById(customerId);
            if (customer == null || customer.IsGuest())
            {
                return RedirectToRoute("HomePage");
            }

            //display "edit" (manage) link
            if (_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel) && _permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                DisplayEditLink(Url.Action("Edit", "Customer", new { id = customer.Id, area = AreaNames.Admin }));

            var model = _profileModelFactory.PrepareProfileIndexModel(customer, pageNumber);
            return View(model);
        }
    }
}