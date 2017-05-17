using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Nop.Admin.Models.Home;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Orders;
using Nop.Services.Security;

namespace Nop.Admin.Components
{
    public class OrderStatisticsViewComponent : ViewComponent
    {
        private readonly IPermissionService _permissionService;
        private readonly IWorkContext _workContext;

        public OrderStatisticsViewComponent(IPermissionService permissionService,
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
