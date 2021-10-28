using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Services.Catalog;
using Nop.Services.Security;

namespace Nop.Web.Areas.Admin.Controllers
{
    public class SearchCompleteController : BaseAdminController
    {
        #region Fields

        protected IPermissionService PermissionService { get; }
        protected IProductService ProductService { get; }
        protected IWorkContext WorkContext { get; }

        #endregion

        #region Ctor

        public SearchCompleteController(
            IPermissionService permissionService,
            IProductService productService,
            IWorkContext workContext)
        {
            PermissionService = permissionService;
            ProductService = productService;
            WorkContext = workContext;
        }

        #endregion

        #region Methods

        public virtual async Task<IActionResult> SearchAutoComplete(string term)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.AccessAdminPanel))
                return Content(string.Empty);

            const int searchTermMinimumLength = 3;
            if (string.IsNullOrWhiteSpace(term) || term.Length < searchTermMinimumLength)
                return Content(string.Empty);

            //a vendor should have access only to his products
            var currentVendor = await WorkContext.GetCurrentVendorAsync();
            var vendorId = 0;
            if (currentVendor != null)
            {
                vendorId = currentVendor.Id;
            }

            //products
            const int productNumber = 15;
            var products = await ProductService.SearchProductsAsync(0,
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