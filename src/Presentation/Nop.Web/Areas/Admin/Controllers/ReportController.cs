using Microsoft.AspNetCore.Mvc;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Models.Reports;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class ReportController : BaseAdminController
    {
        #region Fields

        private readonly IPermissionService _permissionService;
        private readonly IReportModelFactory _reportModelFactory;

        #endregion

        #region Ctor

        public ReportController(
            IPermissionService permissionService,
            IReportModelFactory reportModelFactory)
        {
            _permissionService = permissionService;
            _reportModelFactory = reportModelFactory;
        }

        #endregion

        #region Methods

        #region Low stock

        public virtual IActionResult LowStock()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //prepare model
            var model = _reportModelFactory.PrepareLowStockProductSearchModel(new LowStockProductSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult LowStockList(LowStockProductSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = _reportModelFactory.PrepareLowStockProductListModel(searchModel);

            return Json(model);
        }

        #endregion

        #region Bestsellers

        public virtual IActionResult Bestsellers()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //prepare model
            var model = _reportModelFactory.PrepareBestsellerSearchModel(new BestsellerSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult BestsellersList(BestsellerSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = _reportModelFactory.PrepareBestsellerListModel(searchModel);

            return Json(model);
        }

        [HttpPost]
        public virtual IActionResult BestsellersReportAggregates(BestsellerSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedDataTablesJson();

            //prepare model
            var totalAmount = _reportModelFactory.GetBestsellerTotalAmount(searchModel);

            return Json(new { aggregatortotal = totalAmount });
        }

        #endregion

        #region Never Sold

        public virtual IActionResult NeverSold()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //prepare model
            var model = _reportModelFactory.PrepareNeverSoldSearchModel(new NeverSoldReportSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult NeverSoldList(NeverSoldReportSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = _reportModelFactory.PrepareNeverSoldListModel(searchModel);

            return Json(model);
        }

        #endregion

        #region Country sales

        public virtual IActionResult CountrySales()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.OrderCountryReport))
                return AccessDeniedView();

            //prepare model
            var model = _reportModelFactory.PrepareCountrySalesSearchModel(new CountryReportSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult CountrySalesList(CountryReportSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.OrderCountryReport))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = _reportModelFactory.PrepareCountrySalesListModel(searchModel);

            return Json(model);
        }

        #endregion

        #region Customer reports

        public virtual IActionResult RegisteredCustomers()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            //prepare model
            var model = _reportModelFactory.PrepareCustomerReportsSearchModel(new CustomerReportsSearchModel());

            return View(model);
        }

        public virtual IActionResult BestCustomersByOrderTotal()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            //prepare model
            var model = _reportModelFactory.PrepareCustomerReportsSearchModel(new CustomerReportsSearchModel());

            return View(model);
        }

        public virtual IActionResult BestCustomersByNumberOfOrders()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            //prepare model
            var model = _reportModelFactory.PrepareCustomerReportsSearchModel(new CustomerReportsSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult ReportBestCustomersByOrderTotalList(BestCustomersReportSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = _reportModelFactory.PrepareBestCustomersReportListModel(searchModel);

            return Json(model);
        }

        [HttpPost]
        public virtual IActionResult ReportBestCustomersByNumberOfOrdersList(BestCustomersReportSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = _reportModelFactory.PrepareBestCustomersReportListModel(searchModel);

            return Json(model);
        }

        [HttpPost]
        public virtual IActionResult ReportRegisteredCustomersList(RegisteredCustomersReportSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = _reportModelFactory.PrepareRegisteredCustomersReportListModel(searchModel);

            return Json(model);
        }        

        #endregion

        #endregion
    }
}
