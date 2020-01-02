using System;
using System.Linq;
using Nop.Core.Caching;
using Nop.Plugin.Tax.Avalara.Services;
using Nop.Services.Common;
using Nop.Services.Localization;
using Nop.Services.Tax;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Models.Tax;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Plugin.Tax.Avalara.Factories
{
    /// <summary>
    /// Represents overridden tax model factory
    /// </summary>
    public class OverriddenTaxModelFactory : TaxModelFactory
    {
        #region Fields

        private readonly AvalaraTaxManager _avalaraTaxManager;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IStaticCacheManager _cacheManager;
        private readonly ITaxCategoryService _taxCategoryService;
        private readonly ITaxPluginManager _taxPluginManager;

        #endregion

        #region Ctor

        public OverriddenTaxModelFactory(AvalaraTaxManager avalaraTaxManager,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            IStaticCacheManager cacheManager,
            ITaxCategoryService taxCategoryService,
            ITaxPluginManager taxPluginManager) : base(localizationService,
                taxCategoryService,
                taxPluginManager)
        {
            _avalaraTaxManager = avalaraTaxManager;
            _genericAttributeService = genericAttributeService;
            _cacheManager = cacheManager;
            _taxCategoryService = taxCategoryService;
            _taxPluginManager = taxPluginManager;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare paged tax category list model
        /// </summary>
        /// <param name="searchModel">Tax category search model</param>
        /// <returns>Tax category list model</returns>
        public override TaxCategoryListModel PrepareTaxCategoryListModel(TaxCategorySearchModel searchModel)
        {
            //ensure that Avalara tax provider is active
            if (!_taxPluginManager.IsPluginActive(AvalaraTaxDefaults.SystemName))
                return base.PrepareTaxCategoryListModel(searchModel);

            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get tax categories
            var taxCategories = _taxCategoryService.GetAllTaxCategories().ToPagedList(searchModel);

            //get tax types and define the default value
            var taxTypes = _cacheManager.Get(AvalaraTaxDefaults.TaxCodeTypesCacheKey, () => _avalaraTaxManager.GetTaxCodeTypes())
                ?.Select(taxType => new { Id = taxType.Key, Name = taxType.Value });
            var defaultType = taxTypes
                ?.FirstOrDefault(taxType => taxType.Name.Equals("Unknown", StringComparison.InvariantCultureIgnoreCase))
                ?? taxTypes?.FirstOrDefault();

            //prepare grid model
            var model = new Models.Tax.TaxCategoryListModel().PrepareToGrid(searchModel, taxCategories, () =>
            {
                //fill in model values from the entity
                return taxCategories.Select(taxCategory =>
                {
                    //fill in model values from the entity
                    var taxCategoryModel = new Models.Tax.TaxCategoryModel
                    {
                        Id = taxCategory.Id,
                        Name = taxCategory.Name,
                        DisplayOrder = taxCategory.DisplayOrder
                    };

                    //try to get previously saved tax code type and description
                    var taxCodeType = taxTypes?.FirstOrDefault(type =>
                        type.Id.Equals(_genericAttributeService.GetAttribute<string>(taxCategory, AvalaraTaxDefaults.TaxCodeTypeAttribute) ?? string.Empty))
                        ?? defaultType;
                    taxCategoryModel.Type = taxCodeType?.Name ?? string.Empty;
                    taxCategoryModel.TypeId = taxCodeType?.Id ?? Guid.Empty.ToString();
                    taxCategoryModel.Description = _genericAttributeService
                        .GetAttribute<string>(taxCategory, AvalaraTaxDefaults.TaxCodeDescriptionAttribute) ?? string.Empty;

                    return taxCategoryModel;
                });
            });

            return new TaxCategoryListModel { Data = model.Data, Draw = model.Draw, RecordsTotal = model.RecordsTotal, RecordsFiltered = model.RecordsFiltered };
        }

        #endregion
    }
}