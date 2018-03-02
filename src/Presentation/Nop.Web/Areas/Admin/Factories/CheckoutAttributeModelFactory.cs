using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Orders;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Web.Areas.Admin.Extensions;
using Nop.Web.Areas.Admin.Models.Orders;
using Nop.Web.Framework.Extensions;
using Nop.Web.Framework.Factories;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the checkout attribute model factory implementation
    /// </summary>
    public partial class CheckoutAttributeModelFactory : BaseModelFactory, ICheckoutAttributeModelFactory
    {
        #region Fields

        private readonly CurrencySettings _currencySettings;
        private readonly ICheckoutAttributeParser _checkoutAttributeParser;
        private readonly ICheckoutAttributeService _checkoutAttributeService;
        private readonly ICurrencyService _currencyService;
        private readonly ILocalizationService _localizationService;
        private readonly IMeasureService _measureService;
        private readonly ITaxCategoryService _taxCategoryService;
        private readonly IWorkContext _workContext;
        private readonly MeasureSettings _measureSettings;

        #endregion

        #region Ctor

        public CheckoutAttributeModelFactory(CurrencySettings currencySettings,
            IAclService aclService,
            ICheckoutAttributeParser checkoutAttributeParser,
            ICheckoutAttributeService checkoutAttributeService,
            ICurrencyService currencyService,
            ICustomerService customerService,
            ILanguageService languageService,
            ILocalizationService localizationService,
            IMeasureService measureService,
            IStoreMappingService storeMappingService,
            IStoreService storeService,
            ITaxCategoryService taxCategoryService,
            IWorkContext workContext,
            MeasureSettings measureSettings) : base(aclService,
                customerService,
                languageService,
                storeMappingService,
                storeService)
        {
            this._checkoutAttributeParser = checkoutAttributeParser;
            this._checkoutAttributeService = checkoutAttributeService;
            this._currencyService = currencyService;
            this._currencySettings = currencySettings;
            this._localizationService = localizationService;
            this._measureService = measureService;
            this._measureSettings = measureSettings;
            this._taxCategoryService = taxCategoryService;
            this._workContext = workContext;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Prepare available tax categories for the passed model
        /// </summary>
        /// <param name="model">Checkout attribute model</param>
        /// <param name="checkoutAttribute">Checkout attribute</param>
        protected virtual void PrepareModelTaxCategories(CheckoutAttributeModel model, CheckoutAttribute checkoutAttribute)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            //prepare available tax categories
            var availableTaxCategories = _taxCategoryService.GetAllTaxCategories();
            model.AvailableTaxCategories = availableTaxCategories
                .Select(taxCategory => new SelectListItem { Text = taxCategory.Name, Value = taxCategory.Id.ToString() }).ToList();

            //insert special tax category item for the "none" value
            model.AvailableTaxCategories.Insert(0, new SelectListItem
            {
                Text = _localizationService.GetResource("Admin.Configuration.Settings.Tax.TaxCategories.None"),
                Value = "0"
            });
        }

        /// <summary>
        /// Prepare condition attributes model
        /// </summary>
        /// <param name="model">Condition attributes model</param>
        /// <param name="checkoutAttribute">Checkout attribute</param>
        /// <returns>Condition attributes model</returns>
        protected virtual ConditionModel PrepareConditionAttributesModel(ConditionModel model, CheckoutAttribute checkoutAttribute)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (checkoutAttribute == null)
                throw new ArgumentNullException(nameof(checkoutAttribute));

            model.EnableCondition = !string.IsNullOrEmpty(checkoutAttribute.ConditionAttributeXml);
            if (!model.EnableCondition)
                return model;

            //get selected checkout attribute
            var selectedAttribute = _checkoutAttributeParser.ParseCheckoutAttributes(checkoutAttribute.ConditionAttributeXml).FirstOrDefault();
            model.SelectedAttributeId = selectedAttribute?.Id ?? 0;

            //get selected checkout attribute values identifiers
            var selectedValuesIds = _checkoutAttributeParser
                .ParseCheckoutAttributeValues(checkoutAttribute.ConditionAttributeXml).Select(value => value.Id);

            //get available condition checkout attributes (ignore this attribute and non-combinable attributes)
            var availableConditionAttributes = _checkoutAttributeService.GetAllCheckoutAttributes()
                .Where(attribute => attribute.Id != checkoutAttribute.Id && attribute.CanBeUsedAsCondition());

            model.ConditionAttributes = availableConditionAttributes.Select(attribute => new AttributeConditionModel
            {
                Id = attribute.Id,
                Name = attribute.Name,
                AttributeControlType = attribute.AttributeControlType,
                Values = _checkoutAttributeService.GetCheckoutAttributeValues(attribute.Id).Select(value => new SelectListItem
                {
                    Text = value.Name,
                    Value = value.Id.ToString(),
                    Selected = selectedAttribute?.Id == attribute.Id && selectedValuesIds.Contains(value.Id)
                }).ToList()
            }).ToList();

            return model;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare checkout attribute search model
        /// </summary>
        /// <param name="model">Checkout attribute search model</param>
        /// <returns>Checkout attribute search model</returns>
        public virtual CheckoutAttributeSearchModel PrepareCheckoutAttributeSearchModel(CheckoutAttributeSearchModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            return model;
        }

        /// <summary>
        /// Prepare paged checkout attribute list model
        /// </summary>
        /// <param name="searchModel">Checkout attribute search model</param>
        /// <returns>Checkout attribute list model</returns>
        public virtual CheckoutAttributeListModel PrepareCheckoutAttributeListModel(CheckoutAttributeSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get checkout attributes
            var checkoutAttributes = _checkoutAttributeService.GetAllCheckoutAttributes();

            //prepare list model
            var model = new CheckoutAttributeListModel
            {
                Data = checkoutAttributes.PaginationByRequestModel(searchModel).Select(attribute =>
                {
                    //fill in model values from the entity
                    var attributeModel = attribute.ToModel();

                    //fill in additional values (not existing in the entity)
                    attributeModel.AttributeControlTypeName = attribute.AttributeControlType.GetLocalizedEnum(_localizationService, _workContext);

                    return attributeModel;
                }),
                Total = checkoutAttributes.Count
            };

            return model;
        }

        /// <summary>
        /// Prepare checkout attribute model
        /// </summary>
        /// <param name="model">Checkout attribute model</param>
        /// <param name="checkoutAttribute">Checkout attribute</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>Checkout attribute model</returns>
        public virtual CheckoutAttributeModel PrepareCheckoutAttributeModel(CheckoutAttributeModel model,
            CheckoutAttribute checkoutAttribute, bool excludeProperties = false)
        {
            Action<CheckoutAttributeLocalizedModel, int> localizedModelConfiguration = null;

            if (checkoutAttribute != null)
            {
                //fill in model values from the entity
                model = model ?? checkoutAttribute.ToModel();

                //prepare nested search model
                PrepareCheckoutAttributeValueSearchModel(model.CheckoutAttributeValueSearchModel, checkoutAttribute);

                //define localized model configuration action
                localizedModelConfiguration = (locale, languageId) =>
                {
                    locale.Name = checkoutAttribute.GetLocalized(entity => entity.Name, languageId, false, false);
                    locale.TextPrompt = checkoutAttribute.GetLocalized(entity => entity.TextPrompt, languageId, false, false);
                };

                //whether to fill in some of properties
                if (!excludeProperties)
                    model.TaxCategoryId = checkoutAttribute.TaxCategoryId;

                //prepare condition attributes model
                PrepareConditionAttributesModel(model.ConditionModel, checkoutAttribute);
            }

            //currently any checkout attribute can have condition
            model.ConditionAllowed = true;

            //prepare localized models
            if (!excludeProperties)
                model.Locales = PrepareLocalizedModels(localizedModelConfiguration);

            //prepare model tax categories
            PrepareModelTaxCategories(model, checkoutAttribute);

            //prepare model stores
            PrepareModelStores(model, checkoutAttribute, excludeProperties);

            return model;
        }

        /// <summary>
        /// Prepare checkout attribute value search model
        /// </summary>
        /// <param name="model">Checkout attribute value search model</param>
        /// <param name="checkoutAttribute">Checkout attribute</param>
        /// <returns>Checkout attribute value search model</returns>
        public virtual CheckoutAttributeValueSearchModel PrepareCheckoutAttributeValueSearchModel(CheckoutAttributeValueSearchModel model,
            CheckoutAttribute checkoutAttribute)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            return model;
        }

        /// <summary>
        /// Prepare paged checkout attribute value list model
        /// </summary>
        /// <param name="searchModel">Checkout attribute value search model</param>
        /// <param name="checkoutAttribute">Checkout attribute</param>
        /// <returns>Checkout attribute value list model</returns>
        public virtual CheckoutAttributeValueListModel PrepareCheckoutAttributeValueListModel(CheckoutAttributeValueSearchModel searchModel,
            CheckoutAttribute checkoutAttribute)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (checkoutAttribute == null)
                throw new ArgumentNullException(nameof(checkoutAttribute));

            //get checkout attribute values
            var checkoutAttributeValues = _checkoutAttributeService.GetCheckoutAttributeValues(checkoutAttribute.Id);

            //prepare list model
            var model = new CheckoutAttributeValueListModel
            {
                //fill in model values from the entity
                Data = checkoutAttributeValues.PaginationByRequestModel(searchModel).Select(value => new CheckoutAttributeValueModel
                {
                    Id = value.Id,
                    CheckoutAttributeId = value.CheckoutAttributeId,
                    Name = value.CheckoutAttribute.AttributeControlType != AttributeControlType.ColorSquares
                        ? value.Name : $"{value.Name} - {value.ColorSquaresRgb}",
                    ColorSquaresRgb = value.ColorSquaresRgb,
                    PriceAdjustment = value.PriceAdjustment,
                    WeightAdjustment = value.WeightAdjustment,
                    IsPreSelected = value.IsPreSelected,
                    DisplayOrder = value.DisplayOrder,
                }),
                Total = checkoutAttributeValues.Count
            };

            return model;
        }

        /// <summary>
        /// Prepare checkout attribute value model
        /// </summary>
        /// <param name="model">Checkout attribute value model</param>
        /// <param name="checkoutAttribute">Checkout attribute</param>
        /// <param name="checkoutAttributeValue">Checkout attribute value</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>Checkout attribute value model</returns>
        public virtual CheckoutAttributeValueModel PrepareCheckoutAttributeValueModel(CheckoutAttributeValueModel model,
            CheckoutAttribute checkoutAttribute, CheckoutAttributeValue checkoutAttributeValue, bool excludeProperties = false)
        {
            if (checkoutAttribute == null)
                throw new ArgumentNullException(nameof(checkoutAttribute));

            Action<CheckoutAttributeValueLocalizedModel, int> localizedModelConfiguration = null;

            if (checkoutAttributeValue != null)
            {
                //fill in model values from the entity
                model = new CheckoutAttributeValueModel
                {
                    Name = checkoutAttributeValue.Name,
                    ColorSquaresRgb = checkoutAttributeValue.ColorSquaresRgb,
                    PriceAdjustment = checkoutAttributeValue.PriceAdjustment,
                    WeightAdjustment = checkoutAttributeValue.WeightAdjustment,
                    IsPreSelected = checkoutAttributeValue.IsPreSelected,
                    DisplayOrder = checkoutAttributeValue.DisplayOrder
                };

                //define localized model configuration action
                localizedModelConfiguration = (locale, languageId) =>
                {
                    locale.Name = checkoutAttributeValue.GetLocalized(entity => entity.Name, languageId, false, false);
                };
            }

            model.CheckoutAttributeId = checkoutAttribute.Id;
            model.PrimaryStoreCurrencyCode = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId).CurrencyCode;
            model.BaseWeightIn = _measureService.GetMeasureWeightById(_measureSettings.BaseWeightId).Name;
            model.DisplayColorSquaresRgb = checkoutAttribute.AttributeControlType == AttributeControlType.ColorSquares;

            //prepare localized models
            if (!excludeProperties)
                model.Locales = PrepareLocalizedModels(localizedModelConfiguration);

            return model;
        }

        #endregion
    }
}