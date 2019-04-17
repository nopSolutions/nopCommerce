using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Domain.Vendors;
using Nop.Services.Localization;
using Nop.Services.Vendors;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Vendors;
using Nop.Web.Framework.Factories;
using Nop.Web.Framework.Models.DataTables;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the vendor attribute model factory implementation
    /// </summary>
    public partial class VendorAttributeModelFactory : IVendorAttributeModelFactory
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedModelFactory _localizedModelFactory;
        private readonly IVendorAttributeService _vendorAttributeService;

        #endregion

        #region Ctor

        public VendorAttributeModelFactory(ILocalizationService localizationService,
            ILocalizedModelFactory localizedModelFactory,
            IVendorAttributeService vendorAttributeService)
        {
            _localizationService = localizationService;
            _localizedModelFactory = localizedModelFactory;
            _vendorAttributeService = vendorAttributeService;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Prepare datatables model
        /// </summary>
        /// <param name="searchModel">Search model</param>
        /// <returns>Datatables model</returns>
        protected virtual DataTablesModel PrepareVendorAttributeValueGridModel(VendorAttributeValueSearchModel searchModel)
        {
            //prepare common properties
            var model = new DataTablesModel
            {
                Name = "vendorattributevalues-grid",
                UrlRead = new DataUrl("ValueList", "VendorAttribute", null),
                UrlDelete = new DataUrl("ValueDelete", "VendorAttribute", null),
                Length = searchModel.PageSize,
                LengthMenu = searchModel.AvailablePageSizes
            };

            //prepare filters to search
            model.Filters = new List<FilterParameter>()
            {
                new FilterParameter(nameof(searchModel.VendorAttributeId), searchModel.VendorAttributeId)
            };

            //prepare model columns
            model.ColumnCollection = new List<ColumnProperty>
            {
                new ColumnProperty(nameof(VendorAttributeValueModel.Name))
                {
                    Title = _localizationService.GetResource("Admin.Vendors.VendorAttributes.Values.Fields.Name")
                },
                new ColumnProperty(nameof(VendorAttributeValueModel.IsPreSelected))
                {
                    Title = _localizationService.GetResource("Admin.Vendors.VendorAttributes.Values.Fields.IsPreSelected"),
                    Width = "100",
                    ClassName = StyleColumn.CenterAll,
                    Render = new RenderBoolean()
                },
                new ColumnProperty(nameof(VendorAttributeValueModel.DisplayOrder))
                {
                    Title = _localizationService.GetResource("Admin.Vendors.VendorAttributes.Values.Fields.DisplayOrder"),
                    Width = "100"
                },
                new ColumnProperty(nameof(VendorAttributeValueModel.Id))
                {
                    Title = _localizationService.GetResource("Admin.Common.Edit"),
                    Width = "100",
                    ClassName =  StyleColumn.CenterAll,
                    Render = new RenderCustom("renderColumnEdit")
                },
                new ColumnProperty(nameof(VendorAttributeValueModel.Id))
                {
                    Title = _localizationService.GetResource("Admin.Common.Delete"),
                    Width = "100",
                    Render = new RenderButtonRemove(_localizationService.GetResource("Admin.Common.Delete")) { Style = StyleButton.Default },
                    ClassName =  StyleColumn.CenterAll
                }
            };

            return model;
        }

        /// <summary>
        /// Prepare vendor attribute value search model
        /// </summary>
        /// <param name="searchModel">Vendor attribute value search model</param>
        /// <param name="vendorAttribute">Vendor attribute</param>
        /// <returns>Vendor attribute value search model</returns>
        protected virtual VendorAttributeValueSearchModel PrepareVendorAttributeValueSearchModel(VendorAttributeValueSearchModel searchModel,
            VendorAttribute vendorAttribute)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (vendorAttribute == null)
                throw new ArgumentNullException(nameof(vendorAttribute));

            searchModel.VendorAttributeId = vendorAttribute.Id;

            //prepare page parameters
            searchModel.SetGridPageSize();
            searchModel.Grid = PrepareVendorAttributeValueGridModel(searchModel);

            return searchModel;
        }

        /// <summary>
        /// Prepare datatables model
        /// </summary>
        /// <param name="searchModel">Search model</param>
        /// <returns>Datatables model</returns>
        protected virtual DataTablesModel PrepareVendorAttributeGridModel(VendorAttributeSearchModel searchModel)
        {
            //prepare common properties
            var model = new DataTablesModel
            {
                Name = "vendorattributes-grid",
                UrlRead = new DataUrl("List", "VendorAttribute", null),
                Length = searchModel.PageSize,
                LengthMenu = searchModel.AvailablePageSizes
            };

            //prepare filters to search
            model.Filters = null;

            //prepare model columns
            model.ColumnCollection = new List<ColumnProperty>
            {
                new ColumnProperty(nameof(VendorAttributeModel.Name))
                {
                    Title = _localizationService.GetResource("Admin.Vendors.VendorAttributes.Fields.Name"),
                    Width = "300"
                },
                new ColumnProperty(nameof(VendorAttributeModel.AttributeControlTypeName))
                {
                    Title = _localizationService.GetResource("Admin.Vendors.VendorAttributes.Fields.AttributeControlType"),
                    Width = "200"
                },
                new ColumnProperty(nameof(VendorAttributeModel.IsRequired))
                {
                    Title = _localizationService.GetResource("Admin.Vendors.VendorAttributes.Fields.IsRequired"),
                    Width = "100",
                    ClassName =  StyleColumn.CenterAll,
                    Render = new RenderBoolean()
                },
                new ColumnProperty(nameof(VendorAttributeModel.DisplayOrder))
                {
                    Title = _localizationService.GetResource("Admin.Vendors.VendorAttributes.Fields.DisplayOrder"),
                    Width = "100"
                },
                new ColumnProperty(nameof(VendorAttributeModel.Id))
                {
                    Title = _localizationService.GetResource("Admin.Common.Edit"),
                    Width = "100",
                    ClassName =  StyleColumn.CenterAll,
                    Render = new RenderButtonEdit(new DataUrl("~/Admin/VendorAttribute/Edit/"))
                }
            };

            return model;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare vendor attribute search model
        /// </summary>
        /// <param name="searchModel">Vendor attribute search model</param>
        /// <returns>Vendor attribute search model</returns>
        public virtual VendorAttributeSearchModel PrepareVendorAttributeSearchModel(VendorAttributeSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();
            searchModel.Grid = PrepareVendorAttributeGridModel(searchModel);

            return searchModel;
        }

        /// <summary>
        /// Prepare paged vendor attribute list model
        /// </summary>
        /// <param name="searchModel">Vendor attribute search model</param>
        /// <returns>Vendor attribute list model</returns>
        public virtual VendorAttributeListModel PrepareVendorAttributeListModel(VendorAttributeSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get vendor attributes
            var vendorAttributes = _vendorAttributeService.GetAllVendorAttributes().ToPagedList(searchModel);

            //prepare list model
            var model = new VendorAttributeListModel().PrepareToGrid(searchModel, vendorAttributes, () =>
            {
                return vendorAttributes.Select(attribute =>
                {
                    //fill in model values from the entity
                    var attributeModel = attribute.ToModel<VendorAttributeModel>();

                    //fill in additional values (not existing in the entity)
                    attributeModel.AttributeControlTypeName = _localizationService.GetLocalizedEnum(attribute.AttributeControlType);

                    return attributeModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare vendor attribute model
        /// </summary>
        /// <param name="model">Vendor attribute model</param>
        /// <param name="vendorAttribute">Vendor attribute</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>Vendor attribute model</returns>
        public virtual VendorAttributeModel PrepareVendorAttributeModel(VendorAttributeModel model,
            VendorAttribute vendorAttribute, bool excludeProperties = false)
        {
            Action<VendorAttributeLocalizedModel, int> localizedModelConfiguration = null;

            if (vendorAttribute != null)
            {
                //fill in model values from the entity
                model = model ?? vendorAttribute.ToModel<VendorAttributeModel>();

                //prepare nested search model
                PrepareVendorAttributeValueSearchModel(model.VendorAttributeValueSearchModel, vendorAttribute);

                //define localized model configuration action
                localizedModelConfiguration = (locale, languageId) =>
                {
                    locale.Name = _localizationService.GetLocalized(vendorAttribute, entity => entity.Name, languageId, false, false);
                };
            }

            //prepare localized models
            if (!excludeProperties)
                model.Locales = _localizedModelFactory.PrepareLocalizedModels(localizedModelConfiguration);

            return model;
        }

        /// <summary>
        /// Prepare paged vendor attribute value list model
        /// </summary>
        /// <param name="searchModel">Vendor attribute value search model</param>
        /// <param name="vendorAttribute">Vendor attribute</param>
        /// <returns>Vendor attribute value list model</returns>
        public virtual VendorAttributeValueListModel PrepareVendorAttributeValueListModel(VendorAttributeValueSearchModel searchModel,
            VendorAttribute vendorAttribute)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (vendorAttribute == null)
                throw new ArgumentNullException(nameof(vendorAttribute));

            //get vendor attribute values
            var vendorAttributeValues = _vendorAttributeService.GetVendorAttributeValues(vendorAttribute.Id).ToPagedList(searchModel);

            //prepare list model
            var model = new VendorAttributeValueListModel().PrepareToGrid(searchModel, vendorAttributeValues, () =>
            {
                //fill in model values from the entity
                return vendorAttributeValues.Select(value => value.ToModel<VendorAttributeValueModel>());
            });

            return model;
        }

        /// <summary>
        /// Prepare vendor attribute value model
        /// </summary>
        /// <param name="model">Vendor attribute value model</param>
        /// <param name="vendorAttribute">Vendor attribute</param>
        /// <param name="vendorAttributeValue">Vendor attribute value</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>Vendor attribute value model</returns>
        public virtual VendorAttributeValueModel PrepareVendorAttributeValueModel(VendorAttributeValueModel model,
            VendorAttribute vendorAttribute, VendorAttributeValue vendorAttributeValue, bool excludeProperties = false)
        {
            if (vendorAttribute == null)
                throw new ArgumentNullException(nameof(vendorAttribute));

            Action<VendorAttributeValueLocalizedModel, int> localizedModelConfiguration = null;

            if (vendorAttributeValue != null)
            {
                //fill in model values from the entity
                model = model ?? vendorAttributeValue.ToModel<VendorAttributeValueModel>();

                //define localized model configuration action
                localizedModelConfiguration = (locale, languageId) =>
                {
                    locale.Name = _localizationService.GetLocalized(vendorAttributeValue, entity => entity.Name, languageId, false, false);
                };
            }

            model.VendorAttributeId = vendorAttribute.Id;

            //prepare localized models
            if (!excludeProperties)
                model.Locales = _localizedModelFactory.PrepareLocalizedModels(localizedModelConfiguration);

            return model;
        }

        #endregion
    }
}