using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Services.Security;

namespace Nop.Admin.Components
{
    public class CustomerStatisticsViewComponent : ViewComponent
    {
        private readonly IPermissionService _permissionService;
        private readonly IWorkContext _workContext;

        public CustomerStatisticsViewComponent(IPermissionService permissionService,
            IWorkContext workContext)
        {
            this._permissionService = permissionService;
            this._workContext = workContext;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return Content("");

            //a vendor doesn't have access to this report
            if (_workContext.CurrentVendor != null)
                return Content("");

            return View();
        }
    }
}
