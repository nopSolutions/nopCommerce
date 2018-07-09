using System;
using System.Linq;
using Nop.Core.Domain.Tax;
using Nop.Services.Tax;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Tax;
using Nop.Web.Framework.Extensions;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the tax model factory implementation
    /// </summary>
    public partial class TaxModelFactory : ITaxModelFactory
    {
        #region Fields

        private readonly ITaxCategoryService _taxCategoryService;
        private readonly ITaxService _taxService;
        private readonly TaxSettings _taxSettings;

        #endregion

        #region Ctor

        public TaxModelFactory(ITaxCategoryService taxCategoryService,
            ITaxService taxService,
            TaxSettings taxSettings)
        {
            this._taxCategoryService = taxCategoryService;
            this._taxService = taxService;
            this._taxSettings = taxSettings;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare tax configuration model
        /// </summary>
        /// <param name="taxConfigurationModel">Tax configuration model</param>
        /// <returns>Tax configuration model</returns>
        public virtual TaxConfigurationModel PrepareTaxConfigurationModel(TaxConfigurationModel taxConfigurationModel)
        {
            if (taxConfigurationModel == null)
                throw new ArgumentNullException(nameof(taxConfigurationModel));

            //prepare nested search models
            PrepareTaxProviderSearchModel(taxConfigurationModel.TaxProviders);
            PrepareTaxCategorySearchModel(taxConfigurationModel.TaxCategories);

            return taxConfigurationModel;
        }

        /// <summary>
        /// Prepare tax provider search model
        /// </summary>
        /// <param name="searchModel">Tax provider search model</param>
        /// <returns>Tax provider search model</returns>
        public virtual TaxProviderSearchModel PrepareTaxProviderSearchModel(TaxProviderSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged tax provider list model
        /// </summary>
        /// <param name="searchModel">Tax provider search model</param>
        /// <returns>Tax provider list model</returns>
        public virtual TaxProviderListModel PrepareTaxProviderListModel(TaxProviderSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get tax providers
            var taxProviders = _taxService.LoadAllTaxProviders();

            //prepare grid model
            var model = new TaxProviderListModel
            {
                Data = taxProviders.PaginationByRequestModel(searchModel).Select(provider =>
                {
                    //fill in model values from the entity
                    var taxProviderModel = provider.ToPluginModel<TaxProviderModel>();

                    //fill in additional values (not existing in the entity)
                    taxProviderModel.ConfigurationUrl = provider.GetConfigurationPageUrl();
                    taxProviderModel.IsPrimaryTaxProvider = taxProviderModel.SystemName
                        .Equals(_taxSettings.ActiveTaxProviderSystemName, StringComparison.InvariantCultureIgnoreCase);

                    return taxProviderModel;
                }),
                Total = taxProviders.Count
            };

            return model;
        }

        /// <summary>
        /// Prepare tax category search model
        /// </summary>
        /// <param name="searchModel">Tax category search model</param>
        /// <returns>Tax category search model</returns>
        public virtual TaxCategorySearchModel PrepareTaxCategorySearchModel(TaxCategorySearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged tax category list model
        /// </summary>
        /// <param name="searchModel">Tax category search model</param>
        /// <returns>Tax category list model</returns>
        public virtual TaxCategoryListModel PrepareTaxCategoryListModel(TaxCategorySearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get tax categories
            var taxCategories = _taxCategoryService.GetAllTaxCategories();

            //prepare grid model
            var model = new TaxCategoryListModel
            {
                //fill in model values from the entity
                Data = taxCategories.PaginationByRequestModel(searchModel).Select(taxCategory => taxCategory.ToModel<TaxCategoryModel>()),
                Total = taxCategories.Count
            };

            return model;
        }

        #endregion
    }
}