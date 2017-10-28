using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Services.Catalog;
using Nop.Services.Events;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class ProductReviewController : BaseAdminController
    {
        #region Fields

        private readonly IProductService _productService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IStoreService _storeService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IWorkContext _workContext;

        #endregion Fields

        #region Ctor

        public ProductReviewController(IProductService productService, 
            IDateTimeHelper dateTimeHelper,
            ILocalizationService localizationService, 
            IPermissionService permissionService,
            IEventPublisher eventPublisher,
            IStoreService storeService,
            ICustomerActivityService customerActivityService,
            IWorkContext workContext)
        {
            this._productService = productService;
            this._dateTimeHelper = dateTimeHelper;
            this._localizationService = localizationService;
            this._permissionService = permissionService;
            this._eventPublisher = eventPublisher;
            this._storeService = storeService;
            this._customerActivityService = customerActivityService;
            this._workContext = workContext;
        }

        #endregion

        #region Utilities

        protected virtual void PrepareProductReviewModel(ProductReviewModel model,
            ProductReview productReview, bool excludeProperties, bool formatReviewAndReplyText)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (productReview == null)
                throw new ArgumentNullException(nameof(productReview));

            model.Id = productReview.Id;
            model.StoreName = productReview.Store.Name;
            model.ProductId = productReview.ProductId;
            model.ProductName = productReview.Product.Name;
            model.CustomerId = productReview.CustomerId;
            var customer = productReview.Customer;
            model.CustomerInfo = customer.IsRegistered() ? customer.Email : _localizationService.GetResource("Admin.Customers.Guest");
            model.Rating = productReview.Rating;
            model.CreatedOn = _dateTimeHelper.ConvertToUserTime(productReview.CreatedOnUtc, DateTimeKind.Utc);
            if (!excludeProperties)
            {
                model.Title = productReview.Title;
                if (formatReviewAndReplyText)
                {
                    model.ReviewText = Core.Html.HtmlHelper.FormatText(productReview.ReviewText, false, true, false, false, false, false);
                    model.ReplyText = Core.Html.HtmlHelper.FormatText(productReview.ReplyText, false, true, false, false, false, false);
                }
                else
                {
                    model.ReviewText = productReview.ReviewText;
                    model.ReplyText = productReview.ReplyText;
                }
                model.IsApproved = productReview.IsApproved;
            }

            //a vendor should have access only to his products
            model.IsLoggedInAsVendor = _workContext.CurrentVendor != null;
        }

        #endregion

        #region Methods

        //list
        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual IActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProductReviews))
                return AccessDeniedView();

            var model = new ProductReviewListModel
            {
                //a vendor should have access only to his products
                IsLoggedInAsVendor = _workContext.CurrentVendor != null
            };

            model.AvailableStores.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            var stores = _storeService.GetAllStores().Select(st => new SelectListItem() { Text = st.Name, Value = st.Id.ToString() });
            foreach (var selectListItem in stores)
                model.AvailableStores.Add(selectListItem);

            //"approved" property
            //0 - all
            //1 - approved only
            //2 - disapproved only
            model.AvailableApprovedOptions.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Catalog.ProductReviews.List.SearchApproved.All"), Value = "0" });
            model.AvailableApprovedOptions.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Catalog.ProductReviews.List.SearchApproved.ApprovedOnly"), Value = "1" });
            model.AvailableApprovedOptions.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Catalog.ProductReviews.List.SearchApproved.DisapprovedOnly"), Value = "2" });
            
            return View(model);
        }

        [HttpPost]
        public virtual IActionResult List(DataSourceRequest command, ProductReviewListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProductReviews))
                return AccessDeniedKendoGridJson();

            //a vendor should have access only to his products
            var vendorId = 0;
            if (_workContext.CurrentVendor != null)
            {
                vendorId = _workContext.CurrentVendor.Id;
            }

            var createdOnFromValue = (model.CreatedOnFrom == null) ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.CreatedOnFrom.Value, _dateTimeHelper.CurrentTimeZone);

            var createdToFromValue = (model.CreatedOnTo == null) ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.CreatedOnTo.Value, _dateTimeHelper.CurrentTimeZone).AddDays(1);

            bool? approved = null;
            if (model.SearchApprovedId > 0)
                approved = model.SearchApprovedId == 1;

            var productReviews = _productService.GetAllProductReviews(0, approved, 
                createdOnFromValue, createdToFromValue, model.SearchText, 
                model.SearchStoreId, model.SearchProductId, vendorId, 
                command.Page - 1, command.PageSize);

            var gridModel = new DataSourceResult
            {
                Data = productReviews.Select(x =>
                {
                    var m = new ProductReviewModel();
                    PrepareProductReviewModel(m, x, false, true);
                    return m;
                }),
                Total = productReviews.TotalCount
            };

            return Json(gridModel);
        }

        //edit
        public virtual IActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProductReviews))
                return AccessDeniedView();

            var productReview = _productService.GetProductReviewById(id);
            if (productReview == null)
                //No product review found with the specified id
                return RedirectToAction("List");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && productReview.Product.VendorId != _workContext.CurrentVendor.Id)
                return RedirectToAction("List");

            var model = new ProductReviewModel();
            PrepareProductReviewModel(model, productReview, false, false);
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Edit(ProductReviewModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProductReviews))
                return AccessDeniedView();

            var productReview = _productService.GetProductReviewById(model.Id);
            if (productReview == null)
                //No product review found with the specified id
                return RedirectToAction("List");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && productReview.Product.VendorId != _workContext.CurrentVendor.Id)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                var isLoggedInAsVendor = _workContext.CurrentVendor != null;

                var previousIsApproved = productReview.IsApproved;
                //vendor can edit "Reply text" only
                if (!isLoggedInAsVendor)
                {
                    productReview.Title = model.Title;
                    productReview.ReviewText = model.ReviewText;
                    productReview.IsApproved = model.IsApproved;
                }

                productReview.ReplyText = model.ReplyText;
                _productService.UpdateProduct(productReview.Product);

                //activity log
                _customerActivityService.InsertActivity("EditProductReview", _localizationService.GetResource("ActivityLog.EditProductReview"), productReview.Id);

                //vendor can edit "Reply text" only
                if (!isLoggedInAsVendor)
                {
                    //update product totals
                    _productService.UpdateProductReviewTotals(productReview.Product);

                    //raise event (only if it wasn't approved before and is approved now)
                    if (!previousIsApproved && productReview.IsApproved)
                        _eventPublisher.Publish(new ProductReviewApprovedEvent(productReview));

                }

                SuccessNotification(_localizationService.GetResource("Admin.Catalog.ProductReviews.Updated"));

                return continueEditing ? RedirectToAction("Edit", new { id = productReview.Id }) : RedirectToAction("List");
            }


            //If we got this far, something failed, redisplay form
            PrepareProductReviewModel(model, productReview, true, false);
            return View(model);
        }

        //delete
        [HttpPost]
        public virtual IActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProductReviews))
                return AccessDeniedView();

            var productReview = _productService.GetProductReviewById(id);
            if (productReview == null)
                //No product review found with the specified id
                return RedirectToAction("List");

            //a vendor does not have access to this functionality
            if (_workContext.CurrentVendor != null)
                return RedirectToAction("List");

            var product = productReview.Product;
            _productService.DeleteProductReview(productReview);

            //activity log
            _customerActivityService.InsertActivity("DeleteProductReview", _localizationService.GetResource("ActivityLog.DeleteProductReview"), productReview.Id);

            //update product totals
            _productService.UpdateProductReviewTotals(product);

            SuccessNotification(_localizationService.GetResource("Admin.Catalog.ProductReviews.Deleted"));
            return RedirectToAction("List");
        }

        [HttpPost]
        public virtual IActionResult ApproveSelected(ICollection<int> selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProductReviews))
                return AccessDeniedView();

            //a vendor does not have access to this functionality
            if (_workContext.CurrentVendor != null)
                return RedirectToAction("List");

            if (selectedIds != null)
            {
                //filter not approved reviews
                var productReviews = _productService.GetProducReviewsByIds(selectedIds.ToArray()).Where(review => !review.IsApproved);
                foreach (var productReview in productReviews)
                {
                    productReview.IsApproved = true;
                    _productService.UpdateProduct(productReview.Product);
                    
                    //update product totals
                    _productService.UpdateProductReviewTotals(productReview.Product);

                    //raise event 
                    _eventPublisher.Publish(new ProductReviewApprovedEvent(productReview));
                }
            }

            return Json(new { Result = true });
        }

        [HttpPost]
        public virtual IActionResult DisapproveSelected(ICollection<int> selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProductReviews))
                return AccessDeniedView();

            //a vendor does not have access to this functionality
            if (_workContext.CurrentVendor != null)
                return RedirectToAction("List");

            if (selectedIds != null)
            {
                //filter approved reviews
                var productReviews = _productService.GetProducReviewsByIds(selectedIds.ToArray()).Where(review => review.IsApproved);
                foreach (var productReview in productReviews)
                {
                    productReview.IsApproved = false;
                    _productService.UpdateProduct(productReview.Product);

                    //update product totals
                    _productService.UpdateProductReviewTotals(productReview.Product);
                }
            }

            return Json(new { Result = true });
        }

        [HttpPost]
        public virtual IActionResult DeleteSelected(ICollection<int> selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProductReviews))
                return AccessDeniedView();

            //a vendor does not have access to this functionality
            if (_workContext.CurrentVendor != null)
                return RedirectToAction("List");

            if (selectedIds != null)
            {
                var productReviews = _productService.GetProducReviewsByIds(selectedIds.ToArray());
                var products = _productService.GetProductsByIds(productReviews.Select(p => p.ProductId).Distinct().ToArray());

                _productService.DeleteProductReviews(productReviews);

                //update product totals
                foreach (var product in products)
                {
                    _productService.UpdateProductReviewTotals(product);
                }
            }

            return Json(new { Result = true });
        }

        public virtual IActionResult ProductSearchAutoComplete(string term)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProductReviews))
                return Content("");

            const int searchTermMinimumLength = 3;
            if (string.IsNullOrWhiteSpace(term) || term.Length < searchTermMinimumLength)
                return Content("");

            //a vendor should have access only to his products
            var vendorId = 0;
            if (_workContext.CurrentVendor != null)
            {
                vendorId = _workContext.CurrentVendor.Id;
            }

            //products
            const int productNumber = 15;
            var products = _productService.SearchProducts(
                keywords: term,
                vendorId: vendorId,
                pageSize: productNumber,
                showHidden: true);

            var result = (from p in products
                select new
                {
                    label = p.Name,
                    productid = p.Id
                })
                .ToList();
            return Json(result);
        }

        #endregion
    }
}