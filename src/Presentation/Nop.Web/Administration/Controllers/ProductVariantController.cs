using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nop.Admin.Models;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Media;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Tax;
using Nop.Web.Framework;
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
        private readonly IProductAttributeService _productAttributeService;
        private readonly ITaxCategoryService _taxCategoryService;
        private readonly IWorkContext _workContext;
        #endregion

        #region Constructors

        public ProductVariantController(IProductService productService,
            ILanguageService languageService, ILocalizedEntityService localizedEntityService,
            IDiscountService discountService, ICustomerService customerService,
            ILocalizationService localizationService, IProductAttributeService productAttributeService,
            ITaxCategoryService taxCategoryService, IWorkContext workContext)
        {
            this._localizedEntityService = localizedEntityService;
            this._languageService = languageService;
            this._productService = productService;
            this._discountService = discountService;
            this._customerService = customerService;
            this._localizationService = localizationService;
            this._productAttributeService = productAttributeService;
            this._taxCategoryService = taxCategoryService;
            this._workContext = workContext;
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

        [NonAction]
        private void UpdateAttributeValueLocales(ProductVariantAttributeValue pvav, ProductVariantModel.ProductVariantAttributeValueModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(pvav,
                                                               x => x.Name,
                                                               localized.Name,
                                                               localized.LanguageId);
            }
        }

        [NonAction]
        private void PrepareProductModel(ProductVariantModel model, Product product)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            model.ProductName = product.Name;
        }

        [NonAction]
        private void PrepareProductVariantModel(ProductVariantModel model, ProductVariant variant, bool setPredefinedValues)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            //tax categories
            var taxCategories = _taxCategoryService.GetAllTaxCategories();
            model.AvailableTaxCategories.Add(new SelectListItem() { Text = "---", Value = "0" });
            foreach (var tc in taxCategories)
                model.AvailableTaxCategories.Add(new SelectListItem() { Text = tc.Name, Value = tc.Id.ToString(), Selected = variant != null && !setPredefinedValues && tc.Id == variant.TaxCategoryId });

            //warehouses
            //TODO finish when warehouses are ready
            //var warehouses = _warehouseService.GetAllWarehouses();
            model.AvailableWarehouses.Add(new SelectListItem() { Text = "---", Value = "0" });
            //foreach (var wh in warehouses)
            //    model.AvailableWarehouses.Add(new SelectListItem() { Text = wh.Name, Value = wh.Id.ToString(), Selected = variant != null && !setPredefinedValues && wh.Id == variant.WarehouseId });

            if (setPredefinedValues)
            {
                model.MaximumCustomerEnteredPrice = 1000;
                model.MaxNumberOfDownloads = 10;
                model.RecurringCycleLength = 100;
                model.RecurringTotalCycles = 10;
                model.StockQuantity = 10000;
                model.NotifyAdminForQuantityBelow = 1;
                model.OrderMinimumQuantity = 1;
                model.OrderMaximumQuantity = 10000;
                model.DisplayOrder = 1;

                model.UnlimitedDownloads = true;
                model.IsShipEnabled = true;
                model.Published = true;
            }
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

        [NonAction]
        private void PrepareProductAttributesMapping(ProductVariantModel model)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            model.NumberOfAvailableProductAttributes = _productAttributeService.GetAllProductAttributes().Count;
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
            };
            //locales
            AddLocales(_languageService, model.Locales);
            //common
            PrepareProductModel(model, product);
            PrepareProductVariantModel(model, null, true);
            //attributes
            PrepareProductAttributesMapping(model);
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
                //discounts
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
            //common
            PrepareProductModel(model, product);
            PrepareProductVariantModel(model, null, false);
            //attributes
            PrepareProductAttributesMapping(model);
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
            //locales
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.Name = variant.GetLocalized(x => x.Name, languageId, false);
                locale.Description = variant.GetLocalized(x => x.Description, languageId, false);
            });
            //common
            PrepareProductModel(model, variant.Product);
            PrepareProductVariantModel(model, variant, false);
            //attributes
            PrepareProductAttributesMapping(model);
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
            //common
            PrepareProductModel(model, variant.Product);
            PrepareProductVariantModel(model, variant, false);
            //attributes
            PrepareProductAttributesMapping(model);
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

        public ActionResult LowStockReport()
        {
            var variants = _productService.GetLowStockProductVariants().Take(20).ToList();
            var model = new GridModel<ProductVariantModel>()
            {
                Data = variants.Select(x =>
                {
                    var variantModel = x.ToModel();
                    //Full product variant name
                    variantModel.Name = x.Product.Name + x.Name;
                    return variantModel;
                }),
                Total = variants.Count
            };

            return View(model);
        }
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult LowStockReportList(GridCommand command)
        {
            var variants = _productService.GetLowStockProductVariants();

            var model = new GridModel<ProductVariantModel>()
            {
                Data = variants.PagedForCommand(command).Select(x =>
                {
                    var variantModel = x.ToModel();
                    //Full product variant name
                    variantModel.Name = x.Product.Name + " " + x.Name;
                    return variantModel;
                }),
                Total = variants.Count
            };
            return new JsonResult
            {
                Data = model
            };
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
                        Price1 = x.Price
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
                Price = model.Price1
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
            tierPrice.Price = model.Price1;
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

        #region Product variant attributes

        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult ProductVariantAttributeList(GridCommand command, int productVariantId)
        {
            var productVariantAttributes = _productAttributeService.GetProductVariantAttributesByProductVariantId(productVariantId);
            var productVariantAttributesModel = productVariantAttributes
                .Select(x =>
                {
                    var pvaModel =  new ProductVariantModel.ProductVariantAttributeModel()
                    {
                        Id = x.Id,
                        ProductVariantId = x.ProductVariantId,
                        ProductAttribute = _productAttributeService.GetProductAttributeById(x.ProductAttributeId).Name,
                        ProductAttributeId = x.ProductAttributeId,
                        TextPrompt = x.TextPrompt,
                        IsRequired = x.IsRequired,
                        AttributeControlType = x.AttributeControlType.GetLocalizedEnum(_localizationService, _workContext),
                        AttributeControlTypeId = x.AttributeControlTypeId,
                        DisplayOrder1 = x.DisplayOrder
                    };

                    if (x.ShouldHaveValues())
                    {
                        pvaModel.ViewEditUrl = Url.Action("EditAttributeValues", "ProductVariant", new { productVariantAttributeId = x.Id });
                        pvaModel.ViewEditText = string.Format(_localizationService.GetResource("Admin.Catalog.Products.Variants.ProductVariantAttributes.Attributes.Values.ViewLink"), x.ProductVariantAttributeValues != null ? x.ProductVariantAttributeValues.Count : 0);
                    }
                    return pvaModel;
                })
                .ToList();

            var model = new GridModel<ProductVariantModel.ProductVariantAttributeModel>
            {
                Data = productVariantAttributesModel,
                Total = productVariantAttributesModel.Count
            };

            return new JsonResult
            {
                Data = model
            };
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult ProductVariantAttributeInsert(GridCommand command, ProductVariantModel.ProductVariantAttributeModel model)
        {
            var pva = new ProductVariantAttribute()
            {
                ProductVariantId = model.ProductVariantId,
                ProductAttributeId = Int32.Parse(model.ProductAttribute), //use ProductAttribute property (not ProductAttributeId) because appropriate property is stored in it
                TextPrompt = model.TextPrompt,
                IsRequired = model.IsRequired,
                AttributeControlTypeId = Int32.Parse(model.AttributeControlType), //use AttributeControlType property (not AttributeControlTypeId) because appropriate property is stored in it
                DisplayOrder = model.DisplayOrder1
            };
            _productAttributeService.InsertProductVariantAttribute(pva);

            return ProductVariantAttributeList(command, model.ProductVariantId);
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult ProductVariantAttrbiuteUpdate(GridCommand command, ProductVariantModel.ProductVariantAttributeModel model)
        {
            var pva = _productAttributeService.GetProductVariantAttributeById(model.Id);
            if (pva == null)
                throw new ArgumentException("No product variant attribute found with the specified id");

            //use ProductAttribute property (not ProductAttributeId) because appropriate property is stored in it
            pva.ProductAttributeId = Int32.Parse(model.ProductAttribute);
            pva.TextPrompt = model.TextPrompt;
            pva.IsRequired = model.IsRequired;
            //use AttributeControlType property (not AttributeControlTypeId) because appropriate property is stored in it
            pva.AttributeControlTypeId = Int32.Parse(model.AttributeControlType);
            pva.DisplayOrder = model.DisplayOrder1;
            _productAttributeService.UpdateProductVariantAttribute(pva);

            return ProductVariantAttributeList(command, model.ProductVariantId);
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult ProductVariantAttributeDelete(int id, GridCommand command)
        {
            var pva = _productAttributeService.GetProductVariantAttributeById(id);
            if (pva == null)
                throw new ArgumentException("No product variant attribute found with the specified id");

            var productVariantId = pva.ProductVariantId;
            _productAttributeService.DeleteProductVariantAttribute(pva);

            return ProductVariantAttributeList(command, productVariantId);
        }

        #endregion

        #region Product variant attribute values

        //list
        public ActionResult EditAttributeValues(int productVariantAttributeId)
        {
            var pva = _productAttributeService.GetProductVariantAttributeById(productVariantAttributeId);
            var model = new ProductVariantModel.ProductVariantAttributeValueListModel()
            {
                ProductVariantName = pva.ProductVariant.Product.Name + " " + pva.ProductVariant.Name,
                ProductVariantId = pva.ProductVariantId,
                ProductVariantAttributeName = pva.ProductAttribute.Name,
                ProductVariantAttributeId = pva.Id,
            };

            return View(model);
        }

        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult ProductAttributeValueList(int productVariantAttributeId, GridCommand command)
        {
            var values = _productAttributeService.GetProductVariantAttributeValues(productVariantAttributeId);


            var gridModel = new GridModel<ProductVariantModel.ProductVariantAttributeValueModel>
            {
                Data = values.Select(x =>
                {
                    return new ProductVariantModel.ProductVariantAttributeValueModel()
                    {
                        Id = x.Id,
                        ProductVariantAttributeId = x.ProductVariantAttributeId,
                        Name = x.Name,
                        PriceAdjustment = x.PriceAdjustment,
                        WeightAdjustment = x.WeightAdjustment,
                        IsPreSelected = x.IsPreSelected,
                        DisplayOrder = x.DisplayOrder,
                    };
                }),
                Total = values.Count()
            };
            return new JsonResult
            {
                Data = gridModel
            };
        }

        //delete
        [GridAction(EnableCustomBinding = true)]
        public ActionResult ProductAttributeValueDelete(int pvavId, int productVariantAttributeId, GridCommand command)
        {
            if (!ModelState.IsValid)
            {
                //TODO:Find out how telerik handles errors
                return new JsonResult { Data = "error" };
            }

            var pvav = _productAttributeService.GetProductVariantAttributeValueById(pvavId);
            _productAttributeService.DeleteProductVariantAttributeValue(pvav);

            return ProductAttributeValueList(productVariantAttributeId, command);
        }


        //create
        public ActionResult ProductAttributeValueCreatePopup(int productAttributeAttributeId)
        {
            var model = new ProductVariantModel.ProductVariantAttributeValueModel();
            model.ProductVariantAttributeId = productAttributeAttributeId;
            //locales
            AddLocales(_languageService, model.Locales);
            return View(model);
        }

        [HttpPost]
        public ActionResult ProductAttributeValueCreatePopup(string btnId, ProductVariantModel.ProductVariantAttributeValueModel model)
        {
            var pva = _productAttributeService.GetProductVariantAttributeById(model.ProductVariantAttributeId);
            if (pva == null)
                throw new ArgumentException("No product variant attribute found with the specified id");

            if (ModelState.IsValid)
            {
                var pvav = new ProductVariantAttributeValue()
                {
                    ProductVariantAttributeId = model.ProductVariantAttributeId,
                    Name = model.Name,
                    PriceAdjustment = model.PriceAdjustment,
                    WeightAdjustment = model.WeightAdjustment,
                    IsPreSelected = model.IsPreSelected,
                    DisplayOrder = model.DisplayOrder
                };

                _productAttributeService.InsertProductVariantAttributeValue(pvav);
                UpdateAttributeValueLocales(pvav, model);

                ViewBag.RefreshPage = true;
                ViewBag.btnId = btnId;
                return View(model);
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        //edit
        public ActionResult ProductAttributeValueEditPopup(int id)
        {
            var pvav = _productAttributeService.GetProductVariantAttributeValueById(id);
            if (pvav == null)
                throw new ArgumentException("No attribute value found with the specified id", "id");
            var model = new ProductVariantModel.ProductVariantAttributeValueModel()
            {
                ProductVariantAttributeId = pvav.ProductVariantAttributeId,
                Name= pvav.Name,
                PriceAdjustment = pvav.PriceAdjustment,
                WeightAdjustment = pvav.WeightAdjustment,
                IsPreSelected = pvav.IsPreSelected,
                DisplayOrder = pvav.DisplayOrder
            };
            //locales
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.Name = pvav.GetLocalized(x => x.Name, languageId, false);
            });

            return View(model);
        }

        [HttpPost]
        public ActionResult ProductAttributeValueEditPopup(string btnId, ProductVariantModel.ProductVariantAttributeValueModel model)
        {
            var pvav = _productAttributeService.GetProductVariantAttributeValueById(model.Id);
            if (pvav == null)
                throw new ArgumentException("No attribute value found with the specified id");
            if (ModelState.IsValid)
            {
                pvav.Name = model.Name;
                pvav.PriceAdjustment = model.PriceAdjustment;
                pvav.WeightAdjustment = model.WeightAdjustment;
                pvav.IsPreSelected = model.IsPreSelected;
                pvav.DisplayOrder = model.DisplayOrder;
                _productAttributeService.UpdateProductVariantAttributeValue(pvav);

                UpdateAttributeValueLocales(pvav, model);

                ViewBag.RefreshPage = true;
                ViewBag.btnId = btnId;
                return View(model);
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        #endregion
    }
}
