#if NET451
using System.Web.Mvc;
using Nop.Core.Domain.Customers;
using Nop.Services.Customers;
using Nop.Services.Security;
using Nop.Web.Factories;
using Nop.Web.Framework.Security;

namespace Nop.Web.Controllers
{
    [NopHttpsRequirement(SslRequirement.No)]
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

        public virtual ActionResult Index(int? id, int? page)
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
                DisplayEditLink(Url.Action("Edit", "Customer", new { id = customer.Id, area = "Admin" }));

            var model = _profileModelFactory.PrepareProfileIndexModel(customer, page);
            return View(model);
        }

        //profile info tab
        [ChildActionOnly]
        public virtual ActionResult Info(int customerProfileId)
        {
            var customer = _customerService.GetCustomerById(customerProfileId);
            if (customer == null)
            {
                return RedirectToRoute("HomePage");
            }

            var model = _profileModelFactory.PrepareProfileInfoModel(customer);
            return PartialView(model);
        }

        //latest posts tab
        [ChildActionOnly]
        public virtual ActionResult Posts(int customerProfileId, int page)
        {
            var customer = _customerService.GetCustomerById(customerProfileId);
            if (customer == null)
            {
                return RedirectToRoute("HomePage");
            }
            
            var model = _profileModelFactory.PrepareProfilePostsModel(customer, page);
            return PartialView(model);
        }
    }
}
#endif