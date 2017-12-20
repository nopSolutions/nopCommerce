using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Data;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Models.Home;
using Nop.Web.Framework.Components;
using System.Linq;

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
        private readonly CommonSettings _commonSettings;
        private readonly IDbContext _dbContext;

        public CommonStatisticsViewComponent(IPermissionService permissionService,
            IProductService productService,
            IOrderService orderService,
            ICustomerService customerService,
            IReturnRequestService returnRequestService,
            IWorkContext workContext,
            CommonSettings commonSettings,
            IDbContext dbContext)
        {
            this._permissionService = permissionService;
            this._productService = productService;
            this._orderService = orderService;
            this._customerService = customerService;
            this._returnRequestService = returnRequestService;
            this._workContext = workContext;
            this._commonSettings = commonSettings;
            this._dbContext = dbContext;
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

            CommonStatisticsModel model = null;
            if (!_commonSettings.LargeDatabase)
                model = new CommonStatisticsModel
                {
                    NumberOfOrders = _orderService.SearchOrders(
                pageIndex: 0,
                pageSize: 1).TotalCount,

                    NumberOfCustomers = _customerService.GetAllCustomers(
                customerRoleIds: new[] { _customerService.GetCustomerRoleBySystemName(SystemCustomerRoleNames.Registered).Id },
                pageIndex: 0,
                pageSize: 1).TotalCount,

                    NumberOfPendingReturnRequests = _returnRequestService.SearchReturnRequests(
                rs: ReturnRequestStatus.Pending,
                pageIndex: 0,
                pageSize: 1).TotalCount,

                    NumberOfLowStockProducts = _productService.GetLowStockProducts(0, 0, 1).TotalCount +
                                             _productService.GetLowStockProductCombinations(0, 0, 1).TotalCount
                };
            else
                model = new CommonStatisticsModel
                {
                    NumberOfOrders = _dbContext.Set<Order>().Where(x => !x.Deleted).Count(),

                    NumberOfCustomers = _dbContext.SqlQuery<int>("select count(*) from Customer where Deleted = 0 and Id in (select Customer_Id from Customer_CustomerRole_Mapping where CustomerRole_Id = 3)").First(),

                    NumberOfPendingReturnRequests = _dbContext.Set<ReturnRequest>().Where(x=>x.ReturnRequestStatusId == (int)ReturnRequestStatus.Pending).Count(),

                    NumberOfLowStockProducts = _productService.GetLowStockProductsCount() + _productService.GetLowStockProductCombinationsCount()
                };

            return View(model);
        }
    }
}
