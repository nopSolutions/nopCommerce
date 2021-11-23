using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Models.Reports;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class ReportController : BaseAdminController
    {
        #region Fields

        protected IPermissionService PermissionService { get; }
        protected IReportModelFactory ReportModelFactory { get; }

        #endregion

        #region Ctor

        public ReportController(
            IPermissionService permissionService,
            IReportModelFactory reportModelFactory)
        {
            PermissionService = permissionService;
            ReportModelFactory = reportModelFactory;
        }

        #endregion

        #region Methods

        #region Sales summary

        public virtual async Task<IActionResult> SalesSummary()
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //prepare model
            var model = await ReportModelFactory.PrepareSalesSummarySearchModelAsync(new SalesSummarySearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> SalesSummaryList(SalesSummarySearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.SalesSummaryReport))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await ReportModelFactory.PrepareSalesSummaryListModelAsync(searchModel);

            return Json(model);
        }


        #endregion

        #region Low stock

        public virtual async Task<IActionResult> LowStock()
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //prepare model
            var model = await ReportModelFactory.PrepareLowStockProductSearchModelAsync(new LowStockProductSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> LowStockList(LowStockProductSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await ReportModelFactory.PrepareLowStockProductListModelAsync(searchModel);

            return Json(model);
        }

        #endregion

        #region Bestsellers

        public virtual async Task<IActionResult> Bestsellers()
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //prepare model
            var model = await ReportModelFactory.PrepareBestsellerSearchModelAsync(new BestsellerSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> BestsellersList(BestsellerSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await ReportModelFactory.PrepareBestsellerListModelAsync(searchModel);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> BestsellersReportAggregates(BestsellerSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var totalAmount = await ReportModelFactory.GetBestsellerTotalAmountAsync(searchModel);

            return Json(new { aggregatortotal = totalAmount });
        }

        #endregion

        #region Never Sold

        public virtual async Task<IActionResult> NeverSold()
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //prepare model
            var model = await ReportModelFactory.PrepareNeverSoldSearchModelAsync(new NeverSoldReportSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> NeverSoldList(NeverSoldReportSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await ReportModelFactory.PrepareNeverSoldListModelAsync(searchModel);

            return Json(model);
        }

        #endregion

        #region Country sales

        public virtual async Task<IActionResult> CountrySales()
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.OrderCountryReport))
                return AccessDeniedView();

            //prepare model
            var model = await ReportModelFactory.PrepareCountrySalesSearchModelAsync(new CountryReportSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> CountrySalesList(CountryReportSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.OrderCountryReport))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await ReportModelFactory.PrepareCountrySalesListModelAsync(searchModel);

            return Json(model);
        }

        #endregion

        #region Customer reports

        public virtual async Task<IActionResult> RegisteredCustomers()
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            //prepare model
            var model = await ReportModelFactory.PrepareCustomerReportsSearchModelAsync(new CustomerReportsSearchModel());

            return View(model);
        }

        public virtual async Task<IActionResult> BestCustomersByOrderTotal()
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            //prepare model
            var model = await ReportModelFactory.PrepareCustomerReportsSearchModelAsync(new CustomerReportsSearchModel());

            return View(model);
        }

        public virtual async Task<IActionResult> BestCustomersByNumberOfOrders()
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            //prepare model
            var model = await ReportModelFactory.PrepareCustomerReportsSearchModelAsync(new CustomerReportsSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ReportBestCustomersByOrderTotalList(BestCustomersReportSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageCustomers))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await ReportModelFactory.PrepareBestCustomersReportListModelAsync(searchModel);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ReportBestCustomersByNumberOfOrdersList(BestCustomersReportSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageCustomers))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await ReportModelFactory.PrepareBestCustomersReportListModelAsync(searchModel);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ReportRegisteredCustomersList(RegisteredCustomersReportSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageCustomers))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await ReportModelFactory.PrepareRegisteredCustomersReportListModelAsync(searchModel);

            return Json(model);
        }        

        #endregion

        #endregion
    }
}
