using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Domain.Catalog;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Factories;
using Nop.Web.Framework.Models.DataTables;
using Nop.Web.Framework.Models.Extensions;

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
            _localizationService = localizationService;
            _localizedModelFactory = localizedModelFactory;
            _specificationAttributeService = specificationAttributeService;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Prepare datatables model
        /// </summary>
        /// <param name="searchModel">Search model</param>
        /// <returns>Datatables model</returns>
        protected virtual DataTablesModel PrepareSpecificationAttributeOptionGridModel(SpecificationAttributeOptionSearchModel searchModel)
        {
            //prepare common properties
            var model = new DataTablesModel
            {
                Name = "specificationattributeoptions-grid",
                UrlRead = new DataUrl("OptionList", "SpecificationAttribute", null),
                UrlDelete = new DataUrl("OptionDelete", "SpecificationAttribute", null),
                Length = searchModel.PageSize,
                LengthMenu = searchModel.AvailablePageSizes
            };

            //prepare filters to search
            model.Filters = new List<FilterParameter>
            {
                new FilterParameter(nameof(searchModel.SpecificationAttributeId), searchModel.SpecificationAttributeId)
            };

            //prepare model columns
            model.ColumnCollection = new List<ColumnProperty>
            {
                new ColumnProperty(nameof(SpecificationAttributeOptionModel.Name))
                {
                    Title = _localizationService.GetResource("Admin.Catalog.Attributes.SpecificationAttributes.Options.Fields.Name")
                },
                new ColumnProperty(nameof(SpecificationAttributeOptionModel.DisplayOrder))
                {
                    Title = _localizationService.GetResource("Admin.Catalog.Attributes.SpecificationAttributes.Options.Fields.DisplayOrder"),
                    Width = "100"
                },
                new ColumnProperty(nameof(SpecificationAttributeOptionModel.NumberOfAssociatedProducts))
                {
                    Title = _localizationService.GetResource("Admin.Catalog.Attributes.SpecificationAttributes.Options.Fields.NumberOfAssociatedProducts"),
                    Width = "250"
                },
                new ColumnProperty(nameof(SpecificationAttributeOptionModel.Id))
                {
                    Title = _localizationService.GetResource("Admin.Common.Edit"),
                    Width = "100",
                    ClassName =  StyleColumn.ButtonStyle,
                    Render = new RenderCustom("renderColumnEdit")
                },
                new ColumnProperty(nameof(SpecificationAttributeOptionModel.Id))
                {
                    Title = _localizationService.GetResource("Admin.Common.Delete"),
                    Width = "100",
                    Render = new RenderButtonRemove(_localizationService.GetResource("Admin.Common.Delete")) { Style = StyleButton.Default },
                    ClassName =  StyleColumn.ButtonStyle
                }
            };

            return model;
        }

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
            searchModel.Grid = PrepareSpecificationAttributeOptionGridModel(searchModel);

            return searchModel;
        }

        /// <summary>
        /// Prepare datatables model
        /// </summary>
        /// <param name="searchModel">Search model</param>
        /// <returns>Datatables model</returns>
        protected virtual DataTablesModel PrepareSpecificationAttributeProductGridModel(SpecificationAttributeProductSearchModel searchModel)
        {
            //prepare common properties
            var model = new DataTablesModel
            {
                Name = "used-by-products-grid",
                UrlRead = new DataUrl("UsedByProducts", "SpecificationAttribute", null),
                SearchButtonId = "search-categories",
                Length = searchModel.PageSize,
                LengthMenu = searchModel.AvailablePageSizes
            };

            //prepare filters to search
            model.Filters = new List<FilterParameter>
            {
                new FilterParameter(nameof(searchModel.SpecificationAttributeId), searchModel.SpecificationAttributeId)
            };

            //prepare model columns
            model.ColumnCollection = new List<ColumnProperty>
            {
                new ColumnProperty(nameof(SpecificationAttributeProductModel.ProductName))
                {
                    Title = _localizationService.GetResource("Admin.Catalog.Attributes.SpecificationAttributes.UsedByProducts.Product")
                },
                new ColumnProperty(nameof(SpecificationAttributeProductModel.Published))
                {
                    Title = _localizationService.GetResource("Admin.Catalog.Attributes.SpecificationAttributes.UsedByProducts.Published"),
                    Width = "100",
                    ClassName =  StyleColumn.CenterAll,
                    Render = new RenderBoolean()
                },
                new ColumnProperty(nameof(SpecificationAttributeProductModel.ProductId))
                {
                    Title = _localizationService.GetResource("Admin.Common.View"),
                    Width = "100",
                    ClassName =  StyleColumn.ButtonStyle,
                    Render = new RenderButtonView(new DataUrl("~/Admin/Product/Edit/"))
                }
            };

            return model;
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
            searchModel.Grid = PrepareSpecificationAttributeProductGridModel(searchModel);

            return searchModel;
        }

        /// <summary>
        /// Prepare datatables model
        /// </summary>
        /// <param name="searchModel">Search model</param>
        /// <returns>Datatables model</returns>
        protected virtual DataTablesModel PrepareSpecificationAttributeGridModel(SpecificationAttributeSearchModel searchModel)
        {
            //prepare common properties
            var model = new DataTablesModel
            {
                Name = "specificationattributes-grid",
                UrlRead = new DataUrl("List", "SpecificationAttribute", null),
                Length = searchModel.PageSize,
                LengthMenu = searchModel.AvailablePageSizes
            };

            //prepare model columns
            model.ColumnCollection = new List<ColumnProperty>
            {
                new ColumnProperty(nameof(SpecificationAttributeModel.Name))
                {
                    Title = _localizationService.GetResource("Admin.Catalog.Attributes.SpecificationAttributes.Fields.Name")
                },
                new ColumnProperty(nameof(SpecificationAttributeModel.DisplayOrder))
                {
                    Title = _localizationService.GetResource("Admin.Catalog.Attributes.SpecificationAttributes.Fields.DisplayOrder"),
                    Width = "100"
                },
                new ColumnProperty(nameof(SpecificationAttributeModel.Id))
                {
                    Title = _localizationService.GetResource("Admin.Common.Edit"),
                    Width = "100",
                    ClassName =  StyleColumn.ButtonStyle,
                    Render = new RenderButtonEdit(new DataUrl("Edit"))
                }
            };

            return model;
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
            searchModel.Grid = PrepareSpecificationAttributeGridModel(searchModel);

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
            var options = _specificationAttributeService
                .GetSpecificationAttributeOptionsBySpecificationAttribute(specificationAttribute.Id).ToPagedList(searchModel);

            //prepare list model
            var model = new SpecificationAttributeOptionListModel().PrepareToGrid(searchModel, options, () =>
            {
                return options.Select(option =>
                {
                    //fill in model values from the entity
                    var optionModel = option.ToModel<SpecificationAttributeOptionModel>();

                    //in order to save performance to do not check whether a product is deleted, etc
                    optionModel.NumberOfAssociatedProducts = _specificationAttributeService
                        .GetProductSpecificationAttributeCount(specificationAttributeOptionId: option.Id);

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