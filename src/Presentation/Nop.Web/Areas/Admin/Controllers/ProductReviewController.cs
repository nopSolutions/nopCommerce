using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class ProductReviewController : BaseAdminController
    {
        #region Fields

        private readonly CatalogSettings _catalogSettings;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly IProductReviewModelFactory _productReviewModelFactory;
        private readonly IProductService _productService;
        private readonly IWorkContext _workContext;
        private readonly IWorkflowMessageService _workflowMessageService;

        #endregion Fields

        #region Ctor

        public ProductReviewController(CatalogSettings catalogSettings,
            ICustomerActivityService customerActivityService,
            IEventPublisher eventPublisher,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            INotificationService notificationService,
            IPermissionService permissionService,
            IProductReviewModelFactory productReviewModelFactory,
            IProductService productService,
            IWorkContext workContext,
            IWorkflowMessageService workflowMessageService)
        {
            _catalogSettings = catalogSettings;
            _customerActivityService = customerActivityService;
            _eventPublisher = eventPublisher;
            _genericAttributeService = genericAttributeService;
            _localizationService = localizationService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _productReviewModelFactory = productReviewModelFactory;
            _productService = productService;
            _workContext = workContext;
            _workflowMessageService = workflowMessageService;
        }

        #endregion

        #region Methods

        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual IActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProductReviews))
                return AccessDeniedView();

            //prepare model
            var model = _productReviewModelFactory.PrepareProductReviewSearchModel(new ProductReviewSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult List(ProductReviewSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProductReviews))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = _productReviewModelFactory.PrepareProductReviewListModel(searchModel);

            return Json(model);
        }

        public virtual IActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProductReviews))
                return AccessDeniedView();

            //try to get a product review with the specified id
            var productReview = _productService.GetProductReviewById(id);
            if (productReview == null)
                return RedirectToAction("List");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && _productService.GetProductById(productReview.ProductId).VendorId != _workContext.CurrentVendor.Id)
                return RedirectToAction("List");

            //prepare model
            var model = _productReviewModelFactory.PrepareProductReviewModel(null, productReview);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Edit(ProductReviewModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProductReviews))
                return AccessDeniedView();

            //try to get a product review with the specified id
            var productReview = _productService.GetProductReviewById(model.Id);
            if (productReview == null)
                return RedirectToAction("List");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && _productService.GetProductById(productReview.ProductId).VendorId != _workContext.CurrentVendor.Id)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                var previousIsApproved = productReview.IsApproved;

                //vendor can edit "Reply text" only
                var isLoggedInAsVendor = _workContext.CurrentVendor != null;
                if (!isLoggedInAsVendor)
                {
                    productReview.Title = model.Title;
                    productReview.ReviewText = model.ReviewText;
                    productReview.IsApproved = model.IsApproved;
                }

                productReview.ReplyText = model.ReplyText;

                //notify customer about reply
                if (productReview.IsApproved && !string.IsNullOrEmpty(productReview.ReplyText)
                    && _catalogSettings.NotifyCustomerAboutProductReviewReply && !productReview.CustomerNotifiedOfReply)
                {
                    var customerLanguageId = _genericAttributeService.GetAttribute<Customer, int>(productReview.CustomerId,
                        NopCustomerDefaults.LanguageIdAttribute, productReview.StoreId);

                    var queuedEmailIds = _workflowMessageService.SendProductReviewReplyCustomerNotificationMessage(productReview, customerLanguageId);
                    if (queuedEmailIds.Any())
                        productReview.CustomerNotifiedOfReply = true;
                }

                _productService.UpdateProductReview(productReview);

                //activity log
                _customerActivityService.InsertActivity("EditProductReview",
                   string.Format(_localizationService.GetResource("ActivityLog.EditProductReview"), productReview.Id), productReview);

                //vendor can edit "Reply text" only
                if (!isLoggedInAsVendor)
                {
                    var product = _productService.GetProductById(productReview.ProductId);
                    //update product totals
                    _productService.UpdateProductReviewTotals(product);

                    //raise event (only if it wasn't approved before and is approved now)
                    if (!previousIsApproved && productReview.IsApproved)
                        _eventPublisher.Publish(new ProductReviewApprovedEvent(productReview));
                }

                _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Catalog.ProductReviews.Updated"));

                return continueEditing ? RedirectToAction("Edit", new { id = productReview.Id }) : RedirectToAction("List");
            }

            //prepare model
            model = _productReviewModelFactory.PrepareProductReviewModel(model, productReview, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual IActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProductReviews))
                return AccessDeniedView();

            //try to get a product review with the specified id
            var productReview = _productService.GetProductReviewById(id);
            if (productReview == null)
                return RedirectToAction("List");

            //a vendor does not have access to this functionality
            if (_workContext.CurrentVendor != null)
                return RedirectToAction("List");

            _productService.DeleteProductReview(productReview);

            //activity log
            _customerActivityService.InsertActivity("DeleteProductReview",
                string.Format(_localizationService.GetResource("ActivityLog.DeleteProductReview"), productReview.Id), productReview);

            var product = _productService.GetProductById(productReview.ProductId);

            //update product totals
            _productService.UpdateProductReviewTotals(product);

            _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Catalog.ProductReviews.Deleted"));

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

            if (selectedIds == null)
                return Json(new { Result = true });

            //filter not approved reviews
            var productReviews = _productService.GetProducReviewsByIds(selectedIds.ToArray()).Where(review => !review.IsApproved);
            foreach (var productReview in productReviews)
            {
                productReview.IsApproved = true;
                _productService.UpdateProductReview(productReview);

                var product = _productService.GetProductById(productReview.ProductId);

                //update product totals
                _productService.UpdateProductReviewTotals(product);

                //raise event 
                _eventPublisher.Publish(new ProductReviewApprovedEvent(productReview));
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

            if (selectedIds == null)
                return Json(new { Result = true });

            //filter approved reviews
            var productReviews = _productService.GetProducReviewsByIds(selectedIds.ToArray()).Where(review => review.IsApproved);
            foreach (var productReview in productReviews)
            {
                productReview.IsApproved = false;
                _productService.UpdateProductReview(productReview);

                var product = _productService.GetProductById(productReview.ProductId);

                //update product totals
                _productService.UpdateProductReviewTotals(product);
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

            if (selectedIds == null)
                return Json(new { Result = true });

            var productReviews = _productService.GetProducReviewsByIds(selectedIds.ToArray());
            var products = _productService.GetProductsByIds(productReviews.Select(p => p.ProductId).Distinct().ToArray());

            _productService.DeleteProductReviews(productReviews);

            //update product totals
            foreach (var product in products)
            {
                _productService.UpdateProductReviewTotals(product);
            }

            return Json(new { Result = true });
        }

        [HttpPost]
        public virtual IActionResult ProductReviewReviewTypeMappingList(ProductReviewReviewTypeMappingSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProductReviews))
                return AccessDeniedDataTablesJson();
            var productReview = _productService.GetProductReviewById(searchModel.ProductReviewId)
                ?? throw new ArgumentException("No product review found with the specified id");

            //prepare model
            var model = _productReviewModelFactory.PrepareProductReviewReviewTypeMappingListModel(searchModel, productReview);

            return Json(model);
        }

        #endregion
    }
}