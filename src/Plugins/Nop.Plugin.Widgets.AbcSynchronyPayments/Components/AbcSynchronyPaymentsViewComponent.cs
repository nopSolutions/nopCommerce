using System;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Misc.AbcCore;
using Nop.Plugin.Misc.AbcCore.Infrastructure;
using Nop.Plugin.Misc.AbcCore.Services;
using Nop.Plugin.Widgets.AbcSynchronyPayments.Models;
using Nop.Services.Catalog;
using Nop.Services.Logging;
using Nop.Web.Framework.Components;
using Nop.Web.Models.Catalog;
using Nop.Web.Framework.Infrastructure;
using Nop.Services.Common;
using System.Linq;
using Nop.Plugin.Misc.AbcCore.Mattresses;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.AbcSynchronyPayments.Components
{
    [ViewComponent(Name = "AbcSynchronyPayments")]
    public class AbcSynchronyPaymentsViewComponent : NopViewComponent
    {
        private readonly ILogger _logger;
        private readonly IAbcMattressListingPriceService _abcMattressListingPriceService;
        private readonly IAbcMattressProductService _abcMattressProductService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IProductAbcDescriptionService _productAbcDescriptionService;
        private readonly IProductAbcFinanceService _productAbcFinanceService;
        private readonly IProductService _productService;
        private readonly IStoreContext _storeContext;

        public AbcSynchronyPaymentsViewComponent(
            ILogger logger,
            IGenericAttributeService genericAttributeService,
            IAbcMattressListingPriceService abcMattressListingPriceService,
            IAbcMattressProductService abcMattressProductService,
            IProductAbcDescriptionService productAbcDescriptionService,
            IProductAbcFinanceService productAbcFinanceService,
            IProductService productService,
            IStoreContext storeContext
        )
        {
            _logger = logger;
            _abcMattressListingPriceService = abcMattressListingPriceService;
            _abcMattressProductService = abcMattressProductService;
            _genericAttributeService = genericAttributeService;
            _productAbcDescriptionService = productAbcDescriptionService;
            _productAbcFinanceService = productAbcFinanceService;
            _productService = productService;
            _storeContext = storeContext;
        }

        /*
         * Because of the way AJAX loading of products works, we should always
         * pass in a View instead of an EmptyResult. This ensures the CSS for
         * the modal is always loaded.
         */
        public async Task<IViewComponentResult> InvokeAsync(
            string widgetZone,
            object additionalData = null
        )
        {
            const string productListingCshtml =
                "~/Plugins/Widgets.AbcSynchronyPayments/Views/ProductListing.cshtml";
            SynchronyPaymentModel model = null;

            var productId = GetProductId(additionalData);
            if (productId == -1)
            {
                await _logger.ErrorAsync(
                    "Incorrect data model passed to ABC Warehouse " +
                    "Synchrony Payments widget.");
                return View(productListingCshtml, model);
            }

            var productAbcDescription = await _productAbcDescriptionService.GetProductAbcDescriptionByProductIdAsync(productId);
            var abcItemNumber = productAbcDescription?.AbcItemNumber;

            // also allow getting info from generic attribute
            var productGenericAttributes = await _genericAttributeService.GetAttributesForEntityAsync(productId, "Product");
            var monthsGenericAttribute = productGenericAttributes.Where(ga => ga.Key == "SynchronyPaymentMonths")
                                                                 .FirstOrDefault();

            if (abcItemNumber == null && monthsGenericAttribute == null)
            {
                // No ABC Item number (or no months indicator), skip processing
                return View(productListingCshtml, model);
            }

            var productAbcFinance =
                await _productAbcFinanceService.GetProductAbcFinanceByAbcItemNumberAsync(
                    abcItemNumber
                );
            if (productAbcFinance == null && monthsGenericAttribute == null)
            {
                // No financing information
                return View(productListingCshtml, model);
            }

            // generic attribute data
            var isMinimumGenericAttribute = productGenericAttributes.Where(ga => ga.Key == "SynchronyPaymentIsMinimum")
                                                                 .FirstOrDefault();
            var offerValidFromGenericAttribute = productGenericAttributes.Where(ga => ga.Key == "SynchronyPaymentOfferValidFrom")
                                                                 .FirstOrDefault();
            var offerValidToGenericAttribute = productGenericAttributes.Where(ga => ga.Key == "SynchronyPaymentOfferValidTo")
                                                                 .FirstOrDefault();

            var product = await _productService.GetProductByIdAsync(productId);
            var months = productAbcFinance != null ?
                productAbcFinance.Months :
                int.Parse(monthsGenericAttribute.Value);
            var isMinimumPayment = productAbcFinance != null ?
                productAbcFinance.IsDeferredPricing :
                bool.Parse(isMinimumGenericAttribute.Value);
            var offerValidFrom = productAbcFinance != null ?
                productAbcFinance.StartDate.Value :
                DateTime.Parse(offerValidFromGenericAttribute.Value);
            var offerValidTo = productAbcFinance != null ?
                productAbcFinance.EndDate.Value :
                DateTime.Parse(offerValidToGenericAttribute.Value);
            
            var price = await _abcMattressListingPriceService.GetListingPriceForMattressProductAsync(productId) ?? product.Price;
            var isMattressProduct = _abcMattressProductService.IsMattressProduct(productId);
            
            model = new SynchronyPaymentModel
            {
                MonthCount = months,
                MonthlyPayment = CalculatePayment(
                    price, isMinimumPayment, months
                ),
                ProductId = productId,
                ApplyUrl = await GetApplyUrlAsync(),
                IsMonthlyPaymentStyle = !isMinimumPayment,
                EqualPayment = CalculateEqualPayments(
                    price, months
                ),
                ModalHexColor = await HtmlHelpers.GetPavilionPrimaryColorAsync(),
                StoreName = (await _storeContext.GetCurrentStoreAsync()).Name,
                ImageUrl = await GetImageUrlAsync(),
                OfferValidFrom = offerValidFrom.ToShortDateString(),
                OfferValidTo = offerValidTo.ToShortDateString()
            };

            model.FullPrice = price;
            model.FinalPayment = model.FullPrice -
                (model.MonthlyPayment * (model.MonthCount - 1));
            model.IsHidden = isMattressProduct && price < 697.00M;

            if (model.MonthlyPayment == 0)
            {
                await _logger.WarningAsync(
                    $"ABC Product #{productAbcFinance.AbcItemNumber} has a " +
                    "$0 monthly fee, likely not marked with a correct " +
                    "payment type.");
                return View(productListingCshtml, model);
            }

            switch (widgetZone)
            {
                case "productbox_addinfo_middle":
                    return View(productListingCshtml, model);
                case CustomPublicWidgetZones.ProductDetailsAfterPrice:
                    return View("~/Plugins/Widgets.AbcSynchronyPayments/Views/ProductDetail.cshtml", model);
            }

            await _logger.WarningAsync(
                "ABC Synchrony Payments: Did not match with any passed " +
                "widgets, skipping display");
            return View(productListingCshtml, model);
        }

        private async Task<string> GetImageUrlAsync()
        {
            return (await _storeContext.GetCurrentStoreAsync()).Name == "ABC Warehouse" ?
                "/Plugins/Widgets.AbcSynchronyPayments/Images/deferredPricing.PNG" :
                "/Plugins/Widgets.AbcSynchronyPayments/Images/deferredPricing-haw.PNG";
        }

        private int CalculatePayment(decimal productPrice, bool isMinimumPayment, int months)
        {
            return isMinimumPayment ?
                (int)Math.Max(Math.Round(Math.Ceiling(productPrice * 0.035M), 2), 28) :
                CalculateEqualPayments(productPrice, months);
        }

        private int CalculateEqualPayments(decimal productPrice, int months)
        {
            return (int)Math.Round(Math.Ceiling(productPrice / months), 2);
        }

        private async Task<string> GetApplyUrlAsync()
        {
            return (await _storeContext.GetCurrentStoreAsync()).Name == "ABC Warehouse" ?
                "https://etail.mysynchrony.com/eapply/eapply.action?uniqueId=7EDBAEB071977CE89751663EC89BC474D77B435DDDADAB79&client=ABC%20Warehouse" :
                "https://etail.mysynchrony.com/eapply/eapply.action?uniqueId=7EDBAEB071977CE89751663EC89BC474D77B435DDDADAB79&client=Hawthorne";
        }

        // Gets product ID based on the model passed in
        private int GetProductId(object additionalData)
        {
            if (additionalData is ProductOverviewModel)
            {
                return (additionalData as ProductOverviewModel).Id;
            }
            if (additionalData.GetType() == typeof(int))
            {
                return (int)additionalData;
            }

            return -1;
        }
    }
}
