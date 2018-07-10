using System;
using System.Linq;
using Nop.Core.Domain.Catalog;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Extensions;
using Nop.Web.Framework.Factories;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the specification attribute model factory implementation
    /// </summary>
    public partial class SpecificationAttributeModelFactory : ISpecificationAttributeModelFactory
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedModelFactory _localizedModelFactory;
        private readonly ISpecificationAttributeService _specificationAttributeService;

        #endregion

        #region Ctor

        public SpecificationAttributeModelFactory(ILocalizationService localizationService,
            ILocalizedModelFactory localizedModelFactory,
            ISpecificationAttributeService specificationAttributeService)
        {
            this._localizationService = localizationService;
            this._localizedModelFactory = localizedModelFactory;
            this._specificationAttributeService = specificationAttributeService;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Prepare specification attribute option search model
        /// </summary>
        /// <param name="searchModel">Specification attribute option search model</param>
        /// <param name="specificationAttribute">Specification attribute</param>
        /// <returns>Specification attribute option search model</returns>
        protected virtual SpecificationAttributeOptionSearchModel PrepareSpecificationAttributeOptionSearchModel(
            SpecificationAttributeOptionSearchModel searchModel, SpecificationAttribute specificationAttribute)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (specificationAttribute == null)
                throw new ArgumentNullException(nameof(specificationAttribute));

            searchModel.SpecificationAttributeId = specificationAttribute.Id;

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare search model of products that use the specification attribute
        /// </summary>
        /// <param name="searchModel">Search model of products that use the specification attribute</param>
        /// <param name="specificationAttribute">Specification attribute</param>
        /// <returns>Search model of products that use the specification attribute</returns>
        protected virtual SpecificationAttributeProductSearchModel PrepareSpecificationAttributeProductSearchModel(
            SpecificationAttributeProductSearchModel searchModel, SpecificationAttribute specificationAttribute)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (specificationAttribute == null)
                throw new ArgumentNullException(nameof(specificationAttribute));

            searchModel.SpecificationAttributeId = specificationAttribute.Id;

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare specification attribute search model
        /// </summary>
        /// <param name="searchModel">Specification attribute search model</param>
        /// <returns>Specification attribute search model</returns>
        public virtual SpecificationAttributeSearchModel PrepareSpecificationAttributeSearchModel(SpecificationAttributeSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged specification attribute list model
        /// </summary>
        /// <param name="searchModel">Specification attribute search model</param>
        /// <returns>Specification attribute list model</returns>
        public virtual SpecificationAttributeListModel PrepareSpecificationAttributeListModel(SpecificationAttributeSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get specification attributes
            var specificationAttributes = _specificationAttributeService
                .GetSpecificationAttributes(searchModel.Page - 1, searchModel.PageSize);

            //prepare list model
            var model = new SpecificationAttributeListModel
            {
                //fill in model values from the entity
                Data = specificationAttributes.Select(attribute => attribute.ToModel<SpecificationAttributeModel>()),
                Total = specificationAttributes.TotalCount
            };

            return model;
        }

        /// <summary>
        /// Prepare specification attribute model
        /// </summary>
        /// <param name="model">Specification attribute model</param>
        /// <param name="specificationAttribute">Specification attribute</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>Specification attribute model</returns>
        public virtual SpecificationAttributeModel PrepareSpecificationAttributeModel(SpecificationAttributeModel model,
            SpecificationAttribute specificationAttribute, bool excludeProperties = false)
        {
            Action<SpecificationAttributeLocalizedModel, int> localizedModelConfiguration = null;

            if (specificationAttribute != null)
            {
                //fill in model values from the entity
                model = model ?? specificationAttribute.ToModel<SpecificationAttributeModel>();

                //prepare nested search models
                PrepareSpecificationAttributeOptionSearchModel(model.SpecificationAttributeOptionSearchModel, specificationAttribute);
                PrepareSpecificationAttributeProductSearchModel(model.SpecificationAttributeProductSearchModel, specificationAttribute);

                //define localized model configuration action
                localizedModelConfiguration = (locale, languageId) =>
                {
                    locale.Name = _localizationService.GetLocalized(specificationAttribute, entity => entity.Name, languageId, false, false);
                };
            }

            //prepare localized models
            if (!excludeProperties)
                model.Locales = _localizedModelFactory.PrepareLocalizedModels(localizedModelConfiguration);

            return model;
        }

        /// <summary>
        /// Prepare paged specification attribute option list model
        /// </summary>
        /// <param name="searchModel">Specification attribute option search model</param>
        /// <param name="specificationAttribute">Specification attribute</param>
        /// <returns>Specification attribute option list model</returns>
        public virtual SpecificationAttributeOptionListModel PrepareSpecificationAttributeOptionListModel(
            SpecificationAttributeOptionSearchModel searchModel, SpecificationAttribute specificationAttribute)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (specificationAttribute == null)
                throw new ArgumentNullException(nameof(specificationAttribute));

            //get specification attribute options
            var options = _specificationAttributeService.GetSpecificationAttributeOptionsBySpecificationAttribute(specificationAttribute.Id);

            //prepare list model
            var model = new SpecificationAttributeOptionListModel
            {
                Data = options.PaginationByRequestModel(searchModel).Select(option =>
                {
                    //fill in model values from the entity
                    var optionModel = option.ToModel<SpecificationAttributeOptionModel>();

                    //in order to save performance to do not check whether a product is deleted, etc
                    optionModel.NumberOfAssociatedProducts = _specificationAttributeService
                        .GetProductSpecificationAttributeCount(specificationAttributeOptionId: option.Id);

                    return optionModel;
                }),
                Total = options.Count
            };

            return model;
        }

        /// <summary>
        /// Prepare specification attribute option model
        /// </summary>
        /// <param name="model">Specification attribute option model</param>
        /// <param name="specificationAttribute">Specification attribute</param>
        /// <param name="specificationAttributeOption">Specification attribute option</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>Specification attribute option model</returns>
        public virtual SpecificationAttributeOptionModel PrepareSpecificationAttributeOptionModel(SpecificationAttributeOptionModel model,
            SpecificationAttribute specificationAttribute, SpecificationAttributeOption specificationAttributeOption,
            bool excludeProperties = false)
        {
            if (specificationAttribute == null)
                throw new ArgumentNullException(nameof(specificationAttribute));

            Action<SpecificationAttributeOptionLocalizedModel, int> localizedModelConfiguration = null;

            if (specificationAttributeOption != null)
            {
                //fill in model values from the entity
                model = model ?? specificationAttributeOption.ToModel<SpecificationAttributeOptionModel>();

                model.EnableColorSquaresRgb = !string.IsNullOrEmpty(specificationAttributeOption.ColorSquaresRgb);

                //define localized model configuration action
                localizedModelConfiguration = (locale, languageId) =>
                {
                    locale.Name = _localizationService.GetLocalized(specificationAttributeOption, entity => entity.Name, languageId, false, false);
                };
            }

            model.SpecificationAttributeId = specificationAttribute.Id;

            //prepare localized models
            if (!excludeProperties)
                model.Locales = _localizedModelFactory.PrepareLocalizedModels(localizedModelConfiguration);

            return model;
        }

        /// <summary>
        /// Prepare paged list model of products that use the specification attribute
        /// </summary>
        /// <param name="searchModel">Search model of products that use the specification attribute</param>
        /// <param name="specificationAttribute">Specification attribute</param>
        /// <returns>List model of products that use the specification attribute</returns>
        public virtual SpecificationAttributeProductListModel PrepareSpecificationAttributeProductListModel(
            SpecificationAttributeProductSearchModel searchModel, SpecificationAttribute specificationAttribute)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (specificationAttribute == null)
                throw new ArgumentNullException(nameof(specificationAttribute));

            //get products
            var products = _specificationAttributeService.GetProductsBySpecificationAttributeId(
                specificationAttributeId: specificationAttribute.Id,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare list model
            var model = new SpecificationAttributeProductListModel
            {
                //fill in model values from the entity
                Data = products.Select(product => new SpecificationAttributeProductModel
                {
                    SpecificationAttributeId = specificationAttribute.Id,
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Published = product.Published
                }),
                Total = products.TotalCount
            };

            return model;
        }

        #endregion
    }
}