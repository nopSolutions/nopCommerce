using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nop.Admin.Models;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Discounts;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Web.Framework.Controllers;
using Telerik.Web.Mvc;

namespace Nop.Admin.Controllers
{
    public class ProductVariantController : BaseNopController
    {
        #region Fields

        private readonly IProductService _productService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly IDiscountService _discountService;
        private readonly ICustomerService _customerService;
        private readonly ILocalizationService _localizationService;

        #endregion

        #region Constructors

        public ProductVariantController(IProductService productService,
            ILanguageService languageService, ILocalizedEntityService localizedEntityService,
            IDiscountService discountService, ICustomerService customerService,
            ILocalizationService localizationService)
        {
            this._localizedEntityService = localizedEntityService;
            this._languageService = languageService;
            this._productService = productService;
            this._discountService = discountService;
            this._customerService = customerService;
            this._localizationService = localizationService;
        }
        
        #endregion

        #region Utilities

        [NonAction]
        private void UpdateLocales(ProductVariant variant, ProductVariantModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(variant,
                                                               x => x.Name,
                                                               localized.Name,
                                                               localized.LanguageId);
                _localizedEntityService.SaveLocalizedValue(variant,
                                                               x => x.Description,
                                                               localized.Description,
                                                               localized.LanguageId);
            }
        }

        #endregion

        #region List / Create / Edit / Delete

        public ActionResult Create(int productId)
        {
            var product = _productService.GetProductById(productId);
            if (product == null)
                throw new ArgumentException("No product found with the specified id", "productId");
            var model = new ProductVariantModel()
            {
                ProductId = productId,
                ProductName = product.Name
            };
            //locales
            AddLocales(_languageService, model.Locales);
            //discounts
            PrepareDiscountModel(model, null, true);
            return View(model);
        }

        [HttpPost, FormValueExists("save", "save-continue", "continueEditing")]
        public ActionResult Create(ProductVariantModel model, bool continueEditing)
        {
            if (ModelState.IsValid)
            {
                var variant = model.ToEntity();
                variant.CreatedOnUtc = DateTime.UtcNow;
                variant.UpdatedOnUtc = DateTime.UtcNow;
                //insert variant
                _productService.InsertProductVariant(variant);
                //locales
                UpdateLocales(variant, model);
                //disounts
                var allDiscounts = _discountService.GetAllDiscounts(DiscountType.AssignedToSkus, true);
                foreach (var discount in allDiscounts)
                {
                    if (model.SelectedDiscountIds != null && model.SelectedDiscountIds.Contains(discount.Id))
                        variant.AppliedDiscounts.Add(discount);
                }
                _productService.UpdateProductVariant(variant);

                return continueEditing ? RedirectToAction("Edit", new { id = variant.Id }) : RedirectToAction("Edit", "Product", new { id = variant.ProductId });
            }


            //If we got this far, something failed, redisplay form
            var product = _productService.GetProductById(model.ProductId);
            if (product == null)
                throw new ArgumentException("No product found with the specified id");
            model.ProductName = product.Name;
            //discounts
            PrepareDiscountModel(model, null, true);
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            var variant = _productService.GetProductVariantById(id);
            if (variant == null) 
                throw new ArgumentException("No product variant found with the specified id", "id");
            var model = variant.ToModel();
            model.ProductName = variant.Product.Name;
            //locales
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.Name = variant.GetLocalized(x => x.Name, languageId, false);
                locale.Description = variant.GetLocalized(x => x.Description, languageId, false);
            });
            //discounts
            PrepareDiscountModel(model, variant, false);
            return View(model);
        }

        [HttpPost, FormValueExists("save", "save-continue", "continueEditing")]
        public ActionResult Edit(ProductVariantModel model, bool continueEditing)
        {
            var variant = _productService.GetProductVariantById(model.Id);
            if (variant == null)
                throw new ArgumentException("No product variant found with the specified id");
            if (ModelState.IsValid)
            {
                //UNDONE update
                variant = model.ToEntity(variant);
                variant.UpdatedOnUtc = DateTime.UtcNow;
                //save variant
                _productService.UpdateProductVariant(variant);
                //locales
                UpdateLocales(variant, model);
                //discounts
                var allDiscounts = _discountService.GetAllDiscounts(DiscountType.AssignedToSkus, true);
                foreach (var discount in allDiscounts)
                {
                    if (model.SelectedDiscountIds != null && model.SelectedDiscountIds.Contains(discount.Id))
                    {
                        //new role
                        if (variant.AppliedDiscounts.Where(d => d.Id == discount.Id).Count() == 0)
                            variant.AppliedDiscounts.Add(discount);
                    }
                    else
                    {
                        //removed role
                        if (variant.AppliedDiscounts.Where(d => d.Id == discount.Id).Count() > 0)
                            variant.AppliedDiscounts.Remove(discount);
                    }
                }
                _productService.UpdateProductVariant(variant);

                return continueEditing ? RedirectToAction("Edit", model.Id) : RedirectToAction("Edit", "Product", new { id = variant.ProductId });
            }

            //If we got this far, something failed, redisplay form
            model.ProductName = variant.Product.Name;
            //discounts
            PrepareDiscountModel(model, variant, true);
            return View(model);
        }
        
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            var variant = _productService.GetProductVariantById(id);
            if (variant == null)
                throw new ArgumentException("No product variant found with the specified id");
            var productId = variant.ProductId;
            _productService.DeleteProductVariant(variant);
            return RedirectToAction("Edit", "Product", new { id = productId });
        }
        
        [NonAction]
        private void PrepareDiscountModel(ProductVariantModel model, ProductVariant variant, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            var discounts = _discountService.GetAllDiscounts(DiscountType.AssignedToSkus, true);
            model.AvailableDiscounts = discounts.ToList();

            if (!excludeProperties)
            {
                model.SelectedDiscountIds = variant.AppliedDiscounts.Select(d => d.Id).ToArray();
            }
        }

        #endregion

        #region Tier prices

        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult TierPriceList(GridCommand command, int productVariantId)
        {
            var tierPrices = _productService.GetTierPricesByProductVariantId(productVariantId);
            var tierPricesModel = tierPrices
                .Select(x =>
                {
                    return new ProductVariantModel.TierPriceModel()
                    {
                        Id = x.Id,
                        CustomerRole = x.CustomerRoleId.HasValue ? _customerService.GetCustomerRoleById(x.CustomerRoleId.Value).Name : _localizationService.GetResource("Admin.Catalog.Products.Variants.TierPrices.Fields.CustomerRole.AllRoles"),
                        ProductVariantId = x.ProductVariantId,
                        CustomerRoleId = x.CustomerRoleId.HasValue ? x.CustomerRoleId.Value : 0,
                        Quantity = x.Quantity,
                        Price = x.Price
                    };
                })
                .ToList();

            var model = new GridModel<ProductVariantModel.TierPriceModel>
            {
                Data = tierPricesModel,
                Total = tierPricesModel.Count
            };

            return new JsonResult
            {
                Data = model
            };
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult TierPriceInsert(GridCommand command, ProductVariantModel.TierPriceModel model)
        {
            var tierPrice = new TierPrice()
            {
                ProductVariantId = model.ProductVariantId,
                CustomerRoleId = Int32.Parse(model.CustomerRole) != 0 ? Int32.Parse(model.CustomerRole) : (int?)null, //use CustomerRole property (not CustomerRoleId) because appropriate property is stored in it
                Quantity = model.Quantity,
                Price = model.Price
            };
            _productService.InsertTierPrice(tierPrice);

            return TierPriceList(command, model.ProductVariantId);
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult TierPriceUpdate(GridCommand command, ProductVariantModel.TierPriceModel model)
        {
            var tierPrice = _productService.GetTierPriceById(model.Id);
            if (tierPrice == null)
                throw new ArgumentException("No tier price found with the specified id");

            //use CustomerRole property (not CustomerRoleId) because appropriate property is stored in it
            tierPrice.CustomerRoleId = Int32.Parse(model.CustomerRole) != 0 ? Int32.Parse(model.CustomerRole) : (int?)null;
            tierPrice.Quantity = model.Quantity;
            tierPrice.Price = model.Price;
            _productService.UpdateTierPrice(tierPrice);

            return TierPriceList(command, model.ProductVariantId);
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult TierPriceDelete(int id, GridCommand command)
        {
            var tierPrice = _productService.GetTierPriceById(id);
            if (tierPrice == null)
                throw new ArgumentException("No tier price found with the specified id");

            var productVariantId = tierPrice.ProductVariantId;
            _productService.DeleteTierPrice(tierPrice);

            return TierPriceList(command, productVariantId);
        }

        #endregion
    }
}
