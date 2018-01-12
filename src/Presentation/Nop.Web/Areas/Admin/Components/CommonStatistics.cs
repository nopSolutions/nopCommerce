using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Models.Home;
using Nop.Web.Framework.Components;

namespace Nop.Web.Areas.Admin.Components
{
    public class CommonStatisticsViewComponent : NopViewComponent
    {
        private readonly IPermissionService _permissionService;
        private readonly IProductService _productService;
        private readonly IOrderService _orderService;
        private readonly ICustomerService _customerService;
        private readonly IReturnRequestService _returnRequestService;
        private readonly IWorkContext _workContext;

        public CommonStatisticsViewComponent(IPermissionService permissionService,
            IProductService productService,
            IOrderService orderService,
            ICustomerService customerService,
            IReturnRequestService returnRequestService,
            IWorkContext workContext)
        {
            this._permissionService = permissionService;
            this._productService = productService;
            this._orderService = orderService;
            this._customerService = customerService;
            this._returnRequestService = returnRequestService;
            this._workContext = workContext;
        }

        public IViewComponentResult Invoke()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers) ||
                !_permissionService.Authorize(StandardPermissionProvider.ManageOrders) ||
                !_permissionService.Authorize(StandardPermissionProvider.ManageReturnRequests) ||
                !_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return Content("");

            //a vendor doesn't have access to this report
            if (_workContext.CurrentVendor != null)
                return Content("");

            var model = new CommonStatisticsModel
            {
                NumberOfOrders = _orderService.SearchOrders(
                pageIndex: 0,
                pageSize: 1,
                getOnlyTotalCount: true).TotalCount,

                NumberOfCustomers = _customerService.GetAllCustomers(
                customerRoleIds: new[] { _customerService.GetCustomerRoleBySystemName(SystemCustomerRoleNames.Registered).Id },
                pageIndex: 0,
                pageSize: 1,
                getOnlyTotalCount: true).TotalCount,

                NumberOfPendingReturnRequests = _returnRequestService.SearchReturnRequests(
                rs: ReturnRequestStatus.Pending,
                pageIndex: 0,
                pageSize: 1,
                getOnlyTotalCount: true).TotalCount,

                NumberOfLowStockProducts = 
                    _productService.GetLowStockProducts(getOnlyTotalCount: true).TotalCount +
                    _productService.GetLowStockProductCombinations(getOnlyTotalCount: true).TotalCount
            };

            return View(model);
        }
    }
}
