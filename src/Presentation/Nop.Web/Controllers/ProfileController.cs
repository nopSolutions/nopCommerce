using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Customers;
using Nop.Services.Customers;
using Nop.Services.Security;
using Nop.Web.Factories;
using Nop.Web.Framework;

namespace Nop.Web.Controllers
{
    public partial class ProfileController : BasePublicController
    {
        protected CustomerSettings CustomerSettings { get; }
        protected ICustomerService CustomerService { get; }
        protected IPermissionService PermissionService { get; }
        protected IProfileModelFactory ProfileModelFactory { get; }

        public ProfileController(CustomerSettings customerSettings,
            ICustomerService customerService,
            IPermissionService permissionService,
            IProfileModelFactory profileModelFactory)
        {
            CustomerSettings = customerSettings;
            CustomerService = customerService;
            PermissionService = permissionService;
            ProfileModelFactory = profileModelFactory;
        }

        public virtual async Task<IActionResult> Index(int? id, int? pageNumber)
        {
            if (!CustomerSettings.AllowViewingProfiles)
            {
                return RedirectToRoute("Homepage");
            }

            var customerId = 0;
            if (id.HasValue)
            {
                customerId = id.Value;
            }

            var customer = await CustomerService.GetCustomerByIdAsync(customerId);
            if (customer == null || await CustomerService.IsGuestAsync(customer))
            {
                return RedirectToRoute("Homepage");
            }

            //display "edit" (manage) link
            if (await PermissionService.AuthorizeAsync(StandardPermissionProvider.AccessAdminPanel) && await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageCustomers))
                DisplayEditLink(Url.Action("Edit", "Customer", new { id = customer.Id, area = AreaNames.Admin }));

            var model = await ProfileModelFactory.PrepareProfileIndexModelAsync(customer, pageNumber);
            return View(model);
        }
    }
}