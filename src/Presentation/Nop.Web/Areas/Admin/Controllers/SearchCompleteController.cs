using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Services.Catalog;
using Nop.Services.Security;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class SearchCompleteController : BaseAdminController
    {
        #region Fields

        protected readonly IPermissionService _permissionService;
        protected readonly IProductService _productService;
        protected readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public SearchCompleteController(
            IPermissionService permissionService,
            IProductService productService,
            IWorkContext workContext)
        {
            _permissionService = permissionService;
            _productService = productService;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        public virtual async Task<IActionResult> SearchAutoComplete(string term)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.AccessAdminPanel))
                return Content(string.Empty);

            const int searchTermMinimumLength = 3;
            if (string.IsNullOrWhiteSpace(term) || term.Length < searchTermMinimumLength)
                return Content(string.Empty);

            //a vendor should have access only to his products
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            var vendorId = 0;
            if (currentVendor != null)
            {
                vendorId = currentVendor.Id;
            }

            //products
            const int productNumber = 15;
            var products = await _productService.SearchProductsAsync(0,
                vendorId: vendorId,
                keywords: term,
                pageSize: productNumber,
                showHidden: true);

            var result = (from p in products
                          select new
                          {
                              label = p.Name,
                              productid = p.Id
                          }).ToList();

            return Json(result);
        }

        #endregion
    }
}