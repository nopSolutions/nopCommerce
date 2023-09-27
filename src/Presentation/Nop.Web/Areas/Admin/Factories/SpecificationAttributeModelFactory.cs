using Nop.Core.Domain.Catalog;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Factories;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the specification attribute model factory implementation
    /// </summary>
    public partial class SpecificationAttributeModelFactory : ISpecificationAttributeModelFactory
    {
        #region Fields

        protected readonly IBaseAdminModelFactory _baseAdminModelFactory;
        protected readonly ILocalizationService _localizationService;
        protected readonly ILocalizedModelFactory _localizedModelFactory;
        protected readonly ISpecificationAttributeService _specificationAttributeService;

        #endregion

        #region Ctor

        public SpecificationAttributeModelFactory(IBaseAdminModelFactory baseAdminModelFactory,
            ILocalizationService localizationService,
            ILocalizedModelFactory localizedModelFactory,
            ISpecificationAttributeService specificationAttributeService)
        {
            _baseAdminModelFactory = baseAdminModelFactory;
            _localizationService = localizationService;
            _localizedModelFactory = localizedModelFactory;
            _specificationAttributeService = specificationAttributeService;
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
        /// Prepare specification attribute group search model
        /// </summary>
        /// <param name="searchModel">Specification attribute group search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the specification attribute group search model
        /// </returns>
        public virtual Task<SpecificationAttributeGroupSearchModel> PrepareSpecificationAttributeGroupSearchModelAsync(SpecificationAttributeGroupSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return Task.FromResult(searchModel);
        }

        /// <summary>
        /// Prepare paged specification attribute group list model
        /// </summary>
        /// <param name="searchModel">Specification attribute group search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the specification attribute group list model
        /// </returns>
        public virtual async Task<SpecificationAttributeGroupListModel> PrepareSpecificationAttributeGroupListModelAsync(SpecificationAttributeGroupSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get specification attribute groups
            var specificationAttributeGroups = await _specificationAttributeService
                .GetSpecificationAttributeGroupsAsync(searchModel.Page - 1, searchModel.PageSize);

            if (searchModel.Page == 1)
            {
                //dislpay default group with non-grouped specification attributes on first page
                specificationAttributeGroups.Insert(0, new SpecificationAttributeGroup
                {
                    Name = await _localizationService.GetResourceAsync("Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttributeGroup.DefaultGroupName")
                });
            }

            //prepare list model
            var model = new SpecificationAttributeGroupListModel().PrepareToGrid(searchModel, specificationAttributeGroups, () =>
            {
                //fill in model values from the entity
                return specificationAttributeGroups.Select(attribute => attribute.ToModel<SpecificationAttributeGroupModel>());
            });

            return model;
        }

        /// <summary>
        /// Prepare specification attribute group model
        /// </summary>
        /// <param name="model">Specification attribute group model</param>
        /// <param name="specificationAttributeGroup">Specification attribute group</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the specification attribute group model
        /// </returns>
        public virtual async Task<SpecificationAttributeGroupModel> PrepareSpecificationAttributeGroupModelAsync(SpecificationAttributeGroupModel model,
            SpecificationAttributeGroup specificationAttributeGroup, bool excludeProperties = false)
        {
            Func<SpecificationAttributeGroupLocalizedModel, int, Task> localizedModelConfiguration = null;

            if (specificationAttributeGroup != null)
            {
                //fill in model values from the entity
                model ??= specificationAttributeGroup.ToModel<SpecificationAttributeGroupModel>();

                //define localized model configuration action
                localizedModelConfiguration = async (locale, languageId) =>
                {
                    locale.Name = await _localizationService.GetLocalizedAsync(specificationAttributeGroup, entity => entity.Name, languageId, false, false);
                };
            }

            //prepare localized models
            if (!excludeProperties)
                model.Locales = await _localizedModelFactory.PrepareLocalizedModelsAsync(localizedModelConfiguration);

            return model;
        }

        /// <summary>
        /// Prepare paged specification attribute list model
        /// </summary>
        /// <param name="searchModel">Specification attribute search model</param>
        /// <param name="group">Specification attribute group</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the specification attribute list model
        /// </returns>
        public virtual async Task<SpecificationAttributeListModel> PrepareSpecificationAttributeListModelAsync(SpecificationAttributeSearchModel searchModel, SpecificationAttributeGroup group)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get specification attributes
            var specificationAttributes = (await _specificationAttributeService.GetSpecificationAttributesByGroupIdAsync(group?.Id)).ToPagedList(searchModel);

            //prepare list model
            var model = new SpecificationAttributeListModel().PrepareToGrid(searchModel, specificationAttributes, () =>
            {
                //fill in model values from the entity
                return specificationAttributes.Select(attribute => attribute.ToModel<SpecificationAttributeModel>());
            });

            return model;
        }

        /// <summary>
        /// Prepare specification attribute model
        /// </summary>
        /// <param name="model">Specification attribute model</param>
        /// <param name="specificationAttribute">Specification attribute</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the specification attribute model
        /// </returns>
        public virtual async Task<SpecificationAttributeModel> PrepareSpecificationAttributeModelAsync(SpecificationAttributeModel model,
            SpecificationAttribute specificationAttribute, bool excludeProperties = false)
        {
            Func<SpecificationAttributeLocalizedModel, int, Task> localizedModelConfiguration = null;

            if (specificationAttribute != null)
            {
                //fill in model values from the entity
                model ??= specificationAttribute.ToModel<SpecificationAttributeModel>();

                //prepare nested search models
                PrepareSpecificationAttributeOptionSearchModel(model.SpecificationAttributeOptionSearchModel, specificationAttribute);
                PrepareSpecificationAttributeProductSearchModel(model.SpecificationAttributeProductSearchModel, specificationAttribute);

                //define localized model configuration action
                localizedModelConfiguration = async (locale, languageId) =>
                {
                    locale.Name = await _localizationService.GetLocalizedAsync(specificationAttribute, entity => entity.Name, languageId, false, false);
                };
            }

            //prepare localized models
            if (!excludeProperties)
            {
                model.Locales = await _localizedModelFactory.PrepareLocalizedModelsAsync(localizedModelConfiguration);

                await _baseAdminModelFactory.PrepareSpecificationAttributeGroupsAsync(model.AvailableGroups,
                    defaultItemText: await _localizationService.GetResourceAsync("Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttribute.Fields.SpecificationAttributeGroup.None"));
            }

            return model;
        }

        /// <summary>
        /// Prepare paged specification attribute option list model
        /// </summary>
        /// <param name="searchModel">Specification attribute option search model</param>
        /// <param name="specificationAttribute">Specification attribute</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the specification attribute option list model
        /// </returns>
        public virtual async Task<SpecificationAttributeOptionListModel> PrepareSpecificationAttributeOptionListModelAsync(
            SpecificationAttributeOptionSearchModel searchModel, SpecificationAttribute specificationAttribute)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (specificationAttribute == null)
                throw new ArgumentNullException(nameof(specificationAttribute));

            //get specification attribute options
            var options = (await _specificationAttributeService
                .GetSpecificationAttributeOptionsBySpecificationAttributeAsync(specificationAttribute.Id)).ToPagedList(searchModel);

            //prepare list model
            var model = await new SpecificationAttributeOptionListModel().PrepareToGridAsync(searchModel, options, () =>
            {
                return options.SelectAwait(async option =>
                {
                    //fill in model values from the entity
                    var optionModel = option.ToModel<SpecificationAttributeOptionModel>();

                    //in order to save performance to do not check whether a product is deleted, etc
                    optionModel.NumberOfAssociatedProducts = await _specificationAttributeService
                        .GetProductSpecificationAttributeCountAsync(specificationAttributeOptionId: option.Id);

                    return optionModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare specification attribute option model
        /// </summary>
        /// <param name="model">Specification attribute option model</param>
        /// <param name="specificationAttribute">Specification attribute</param>
        /// <param name="specificationAttributeOption">Specification attribute option</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the specification attribute option model
        /// </returns>
        public virtual async Task<SpecificationAttributeOptionModel> PrepareSpecificationAttributeOptionModelAsync(SpecificationAttributeOptionModel model,
            SpecificationAttribute specificationAttribute, SpecificationAttributeOption specificationAttributeOption,
            bool excludeProperties = false)
        {
            if (specificationAttribute == null)
                throw new ArgumentNullException(nameof(specificationAttribute));

            Func<SpecificationAttributeOptionLocalizedModel, int, Task> localizedModelConfiguration = null;

            if (specificationAttributeOption != null)
            {
                //fill in model values from the entity
                model ??= specificationAttributeOption.ToModel<SpecificationAttributeOptionModel>();

                model.EnableColorSquaresRgb = !string.IsNullOrEmpty(specificationAttributeOption.ColorSquaresRgb);

                //define localized model configuration action
                localizedModelConfiguration = async (locale, languageId) =>
                {
                    locale.Name = await _localizationService.GetLocalizedAsync(specificationAttributeOption, entity => entity.Name, languageId, false, false);
                };
            }

            model.SpecificationAttributeId = specificationAttribute.Id;

            //prepare localized models
            if (!excludeProperties)
                model.Locales = await _localizedModelFactory.PrepareLocalizedModelsAsync(localizedModelConfiguration);

            return model;
        }

        /// <summary>
        /// Prepare paged list model of products that use the specification attribute
        /// </summary>
        /// <param name="searchModel">Search model of products that use the specification attribute</param>
        /// <param name="specificationAttribute">Specification attribute</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list model of products that use the specification attribute
        /// </returns>
        public virtual async Task<SpecificationAttributeProductListModel> PrepareSpecificationAttributeProductListModelAsync(
            SpecificationAttributeProductSearchModel searchModel, SpecificationAttribute specificationAttribute)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (specificationAttribute == null)
                throw new ArgumentNullException(nameof(specificationAttribute));

            //get products
            var products = await _specificationAttributeService.GetProductsBySpecificationAttributeIdAsync(
                specificationAttributeId: specificationAttribute.Id,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare list model
            var model = new SpecificationAttributeProductListModel().PrepareToGrid(searchModel, products, () =>
            {
                //fill in model values from the entity
                return products.Select(product =>
                {
                    var specificationAttributeProductModel = product.ToModel<SpecificationAttributeProductModel>();
                    specificationAttributeProductModel.ProductId = product.Id;
                    specificationAttributeProductModel.ProductName = product.Name;
                    specificationAttributeProductModel.SpecificationAttributeId = specificationAttribute.Id;

                    return specificationAttributeProductModel;
                });
            });

            return model;
        }

        #endregion
    }
}