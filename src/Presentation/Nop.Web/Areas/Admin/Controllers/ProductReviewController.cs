using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Events;
using Nop.Services.Catalog;
using Nop.Services.Common;
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

        protected CatalogSettings CatalogSettings { get; }
        protected ICustomerActivityService CustomerActivityService { get; }
        protected IEventPublisher EventPublisher { get; }
        protected IGenericAttributeService GenericAttributeService { get; }
        protected ILocalizationService LocalizationService { get; }
        protected INotificationService NotificationService { get; }
        protected IPermissionService PermissionService { get; }
        protected IProductReviewModelFactory ProductReviewModelFactory { get; }
        protected IProductService ProductService { get; }
        protected IWorkContext WorkContext { get; }
        protected IWorkflowMessageService WorkflowMessageService { get; }

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
            CatalogSettings = catalogSettings;
            CustomerActivityService = customerActivityService;
            EventPublisher = eventPublisher;
            GenericAttributeService = genericAttributeService;
            LocalizationService = localizationService;
            NotificationService = notificationService;
            PermissionService = permissionService;
            ProductReviewModelFactory = productReviewModelFactory;
            ProductService = productService;
            WorkContext = workContext;
            WorkflowMessageService = workflowMessageService;
        }

        #endregion

        #region Methods

        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual async Task<IActionResult> List()
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageProductReviews))
                return AccessDeniedView();

            //prepare model
            var model = await ProductReviewModelFactory.PrepareProductReviewSearchModelAsync(new ProductReviewSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> List(ProductReviewSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageProductReviews))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await ProductReviewModelFactory.PrepareProductReviewListModelAsync(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageProductReviews))
                return AccessDeniedView();

            //try to get a product review with the specified id
            var productReview = await ProductService.GetProductReviewByIdAsync(id);
            if (productReview == null)
                return RedirectToAction("List");

            //a vendor should have access only to his products
            var currentVendor = await WorkContext.GetCurrentVendorAsync();
            if (currentVendor != null && (await ProductService.GetProductByIdAsync(productReview.ProductId)).VendorId != currentVendor.Id)
                return RedirectToAction("List");

            //prepare model
            var model = await ProductReviewModelFactory.PrepareProductReviewModelAsync(null, productReview);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Edit(ProductReviewModel model, bool continueEditing)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageProductReviews))
                return AccessDeniedView();

            //try to get a product review with the specified id
            var productReview = await ProductService.GetProductReviewByIdAsync(model.Id);
            if (productReview == null)
                return RedirectToAction("List");

            //a vendor should have access only to his products
            var currentVendor = await WorkContext.GetCurrentVendorAsync();
            if (currentVendor != null && (await ProductService.GetProductByIdAsync(productReview.ProductId)).VendorId != currentVendor.Id)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                var previousIsApproved = productReview.IsApproved;

                //vendor can edit "Reply text" only
                var isLoggedInAsVendor = currentVendor != null;
                if (!isLoggedInAsVendor)
                {
                    productReview.Title = model.Title;
                    productReview.ReviewText = model.ReviewText;
                    productReview.IsApproved = model.IsApproved;
                }

                productReview.ReplyText = model.ReplyText;

                //notify customer about reply
                if (productReview.IsApproved && !string.IsNullOrEmpty(productReview.ReplyText)
                    && CatalogSettings.NotifyCustomerAboutProductReviewReply && !productReview.CustomerNotifiedOfReply)
                {
                    var customerLanguageId = await GenericAttributeService.GetAttributeAsync<Customer, int>(productReview.CustomerId,
                        NopCustomerDefaults.LanguageIdAttribute, productReview.StoreId);

                    var queuedEmailIds = await WorkflowMessageService.SendProductReviewReplyCustomerNotificationMessageAsync(productReview, customerLanguageId);
                    if (queuedEmailIds.Any())
                        productReview.CustomerNotifiedOfReply = true;
                }

                await ProductService.UpdateProductReviewAsync(productReview);

                //activity log
                await CustomerActivityService.InsertActivityAsync("EditProductReview",
                   string.Format(await LocalizationService.GetResourceAsync("ActivityLog.EditProductReview"), productReview.Id), productReview);

                //vendor can edit "Reply text" only
                if (!isLoggedInAsVendor)
                {
                    var product = await ProductService.GetProductByIdAsync(productReview.ProductId);
                    //update product totals
                    await ProductService.UpdateProductReviewTotalsAsync(product);

                    //raise event (only if it wasn't approved before and is approved now)
                    if (!previousIsApproved && productReview.IsApproved)
                        await EventPublisher.PublishAsync(new ProductReviewApprovedEvent(productReview));
                }

                NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Catalog.ProductReviews.Updated"));

                return continueEditing ? RedirectToAction("Edit", new { id = productReview.Id }) : RedirectToAction("List");
            }

            //prepare model
            model = await ProductReviewModelFactory.PrepareProductReviewModelAsync(model, productReview, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageProductReviews))
                return AccessDeniedView();

            //try to get a product review with the specified id
            var productReview = await ProductService.GetProductReviewByIdAsync(id);
            if (productReview == null)
                return RedirectToAction("List");

            //a vendor does not have access to this functionality
            if (await WorkContext.GetCurrentVendorAsync() != null)
                return RedirectToAction("List");

            await ProductService.DeleteProductReviewAsync(productReview);

            //activity log
            await CustomerActivityService.InsertActivityAsync("DeleteProductReview",
                string.Format(await LocalizationService.GetResourceAsync("ActivityLog.DeleteProductReview"), productReview.Id), productReview);

            var product = await ProductService.GetProductByIdAsync(productReview.ProductId);

            //update product totals
            await ProductService.UpdateProductReviewTotalsAsync(product);

            NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Catalog.ProductReviews.Deleted"));

            return RedirectToAction("List");
        }

        [HttpPost]
        public virtual async Task<IActionResult> ApproveSelected(ICollection<int> selectedIds)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageProductReviews))
                return AccessDeniedView();

            //a vendor does not have access to this functionality
            if (await WorkContext.GetCurrentVendorAsync() != null)
                return RedirectToAction("List");

            if (selectedIds == null || selectedIds.Count() == 0)
                return NoContent();

            //filter not approved reviews
            var productReviews = (await ProductService.GetProductReviewsByIdsAsync(selectedIds.ToArray())).Where(review => !review.IsApproved);

            foreach (var productReview in productReviews)
            {
                productReview.IsApproved = true;
                await ProductService.UpdateProductReviewAsync(productReview);

                var product = await ProductService.GetProductByIdAsync(productReview.ProductId);

                //update product totals
                await ProductService.UpdateProductReviewTotalsAsync(product);

                //raise event 
                await EventPublisher.PublishAsync(new ProductReviewApprovedEvent(productReview));
            }

            return Json(new { Result = true });
        }

        [HttpPost]
        public virtual async Task<IActionResult> DisapproveSelected(ICollection<int> selectedIds)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageProductReviews))
                return AccessDeniedView();

            //a vendor does not have access to this functionality
            if (await WorkContext.GetCurrentVendorAsync() != null)
                return RedirectToAction("List");

            if (selectedIds == null || selectedIds.Count() == 0)
                return NoContent();

            //filter approved reviews
            var productReviews = (await ProductService.GetProductReviewsByIdsAsync(selectedIds.ToArray())).Where(review => review.IsApproved);

            foreach (var productReview in productReviews)
            {
                productReview.IsApproved = false;
                await ProductService.UpdateProductReviewAsync(productReview);

                var product = await ProductService.GetProductByIdAsync(productReview.ProductId);

                //update product totals
                await ProductService.UpdateProductReviewTotalsAsync(product);
            }

            return Json(new { Result = true });
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteSelected(ICollection<int> selectedIds)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageProductReviews))
                return AccessDeniedView();

            //a vendor does not have access to this functionality
            if (await WorkContext.GetCurrentVendorAsync() != null)
                return RedirectToAction("List");

            if (selectedIds == null || selectedIds.Count() == 0)
                return NoContent();

            var productReviews = await ProductService.GetProductReviewsByIdsAsync(selectedIds.ToArray());
            var products = await ProductService.GetProductsByIdsAsync(productReviews.Select(p => p.ProductId).Distinct().ToArray());

            await ProductService.DeleteProductReviewsAsync(productReviews);

            //update product totals
            foreach (var product in products)
            {
                await ProductService.UpdateProductReviewTotalsAsync(product);
            }

            return Json(new { Result = true });
        }

        [HttpPost]
        public virtual async Task<IActionResult> ProductReviewReviewTypeMappingList(ProductReviewReviewTypeMappingSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageProductReviews))
                return await AccessDeniedDataTablesJson();
            var productReview = await ProductService.GetProductReviewByIdAsync(searchModel.ProductReviewId)
                ?? throw new ArgumentException("No product review found with the specified id");

            //prepare model
            var model = await ProductReviewModelFactory.PrepareProductReviewReviewTypeMappingListModelAsync(searchModel, productReview);

            return Json(model);
        }

        #endregion
    }
}