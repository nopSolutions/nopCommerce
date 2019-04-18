using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Domain.Customers;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Customers;
using Nop.Web.Framework.Factories;
using Nop.Web.Framework.Models.DataTables;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the customer attribute model factory implementation
    /// </summary>
    public partial class CustomerAttributeModelFactory : ICustomerAttributeModelFactory
    {
        #region Fields

        private readonly ICustomerAttributeService _customerAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedModelFactory _localizedModelFactory;

        #endregion

        #region Ctor

        public CustomerAttributeModelFactory(ICustomerAttributeService customerAttributeService,
            ILocalizationService localizationService,
            ILocalizedModelFactory localizedModelFactory)
        {
            _customerAttributeService = customerAttributeService;
            _localizationService = localizationService;
            _localizedModelFactory = localizedModelFactory;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Prepare datatables model
        /// </summary>
        /// <param name="searchModel">Search model</param>
        /// <returns>Datatables model</returns>
        protected virtual DataTablesModel PrepareCustomerAttributeValueGridModel(CustomerAttributeValueSearchModel searchModel)
        {
            //prepare common properties
            var model = new DataTablesModel
            {
                Name = "customerattributevalues-grid",
                UrlRead = new DataUrl("ValueList", "CustomerAttribute", null),
                UrlDelete = new DataUrl("ValueDelete", "CustomerAttribute", null),
                Length = searchModel.PageSize,
                LengthMenu = searchModel.AvailablePageSizes
            };

            //prepare filters to search
            model.Filters = new List<FilterParameter>
            {
                new FilterParameter(nameof(searchModel.CustomerAttributeId), searchModel.CustomerAttributeId)
            };

            //prepare model columns
            model.ColumnCollection = new List<ColumnProperty>
            {
                new ColumnProperty(nameof(CustomerAttributeValueModel.Name))
                {
                    Title = _localizationService.GetResource("Admin.Customers.CustomerAttributes.Values.Fields.Name")
                },
                new ColumnProperty(nameof(CustomerAttributeValueModel.IsPreSelected))
                {
                    Title = _localizationService.GetResource("Admin.Customers.CustomerAttributes.Values.Fields.IsPreSelected"),
                    Width = "100",
                    ClassName = StyleColumn.CenterAll,
                    Render = new RenderBoolean()
                },
                new ColumnProperty(nameof(CustomerAttributeValueModel.DisplayOrder))
                {
                    Title = _localizationService.GetResource("Admin.Customers.CustomerAttributes.Values.Fields.DisplayOrder"),
                    Width = "100"
                },
                new ColumnProperty(nameof(CustomerAttributeValueModel.Id))
                {
                    Title = _localizationService.GetResource("Admin.Common.Edit"),
                    Width = "100",
                    ClassName =  StyleColumn.ButtonStyle,
                    Render = new RenderCustom("renderColumnEdit")
                },
                new ColumnProperty(nameof(CustomerAttributeValueModel.Id))
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
        /// Prepare customer attribute value search model
        /// </summary>
        /// <param name="searchModel">Customer attribute value search model</param>
        /// <param name="customerAttribute">Customer attribute</param>
        /// <returns>Customer attribute value search model</returns>
        protected virtual CustomerAttributeValueSearchModel PrepareCustomerAttributeValueSearchModel(CustomerAttributeValueSearchModel searchModel,
            CustomerAttribute customerAttribute)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (customerAttribute == null)
                throw new ArgumentNullException(nameof(customerAttribute));

            searchModel.CustomerAttributeId = customerAttribute.Id;

            //prepare page parameters
            searchModel.SetGridPageSize();
            searchModel.Grid = PrepareCustomerAttributeValueGridModel(searchModel);

            return searchModel;
        }


        /// <summary>
        /// Prepare datatables model
        /// </summary>
        /// <param name="searchModel">Search model</param>
        /// <returns>Datatables model</returns>
        protected virtual DataTablesModel PrepareCustomerAttributeGridModel(CustomerAttributeSearchModel searchModel)
        {
            //prepare common properties
            var model = new DataTablesModel
            {
                Name = "customerattributes-grid",
                UrlRead = new DataUrl("List", "CustomerAttribute", null),
                Length = searchModel.PageSize,
                LengthMenu = searchModel.AvailablePageSizes
            };

            //prepare model columns
            model.ColumnCollection = new List<ColumnProperty>
            {
                new ColumnProperty(nameof(CustomerAttributeModel.Name))
                {
                    Title = _localizationService.GetResource("Admin.Customers.CustomerAttributes.Fields.Name"),
                    Width = "300"
                },
                new ColumnProperty(nameof(CustomerAttributeModel.AttributeControlTypeName))
                {
                    Title = _localizationService.GetResource("Admin.Customers.CustomerAttributes.Fields.AttributeControlType"),
                    Width = "200"
                },
                new ColumnProperty(nameof(CustomerAttributeModel.IsRequired))
                {
                    Title = _localizationService.GetResource("Admin.Customers.CustomerAttributes.Fields.IsRequired"),
                    Width = "200",
                    ClassName =  StyleColumn.CenterAll,
                    Render = new RenderBoolean()
                },
                new ColumnProperty(nameof(CustomerAttributeModel.DisplayOrder))
                {
                    Title = _localizationService.GetResource("Admin.Customers.CustomerAttributes.Fields.DisplayOrder"),
                    Width = "100",
                    ClassName =  StyleColumn.CenterAll
                },
                new ColumnProperty(nameof(CustomerAttributeModel.Id))
                {
                    Title = _localizationService.GetResource("Admin.Common.Edit"),
                    Width = "100",
                    ClassName =  StyleColumn.ButtonStyle,
                    Render = new RenderButtonEdit(new DataUrl("~/Admin/CustomerAttribute/Edit/"))
                }
            };

            return model;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare customer attribute search model
        /// </summary>
        /// <param name="searchModel">Customer attribute search model</param>
        /// <returns>Customer attribute search model</returns>
        public virtual CustomerAttributeSearchModel PrepareCustomerAttributeSearchModel(CustomerAttributeSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();
            searchModel.Grid = PrepareCustomerAttributeGridModel(searchModel);

            return searchModel;
        }

        /// <summary>
        /// Prepare paged customer attribute list model
        /// </summary>
        /// <param name="searchModel">Customer attribute search model</param>
        /// <returns>Customer attribute list model</returns>
        public virtual CustomerAttributeListModel PrepareCustomerAttributeListModel(CustomerAttributeSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get customer attributes
            var customerAttributes = _customerAttributeService.GetAllCustomerAttributes().ToPagedList(searchModel);

            //prepare list model
            var model = new CustomerAttributeListModel().PrepareToGrid(searchModel, customerAttributes, () =>
            {
                return customerAttributes.Select(attribute =>
                {
                    //fill in model values from the entity
                    var attributeModel = attribute.ToModel<CustomerAttributeModel>();

                    //fill in additional values (not existing in the entity)
                    attributeModel.AttributeControlTypeName = _localizationService.GetLocalizedEnum(attribute.AttributeControlType);

                    return attributeModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare customer attribute model
        /// </summary>
        /// <param name="model">Customer attribute model</param>
        /// <param name="customerAttribute">Customer attribute</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>Customer attribute model</returns>
        public virtual CustomerAttributeModel PrepareCustomerAttributeModel(CustomerAttributeModel model,
            CustomerAttribute customerAttribute, bool excludeProperties = false)
        {
            Action<CustomerAttributeLocalizedModel, int> localizedModelConfiguration = null;

            if (customerAttribute != null)
            {
                //fill in model values from the entity
                model = model ?? customerAttribute.ToModel<CustomerAttributeModel>();

                //prepare nested search model
                PrepareCustomerAttributeValueSearchModel(model.CustomerAttributeValueSearchModel, customerAttribute);

                //define localized model configuration action
                localizedModelConfiguration = (locale, languageId) =>
                {
                    locale.Name = _localizationService.GetLocalized(customerAttribute, entity => entity.Name, languageId, false, false);
                };
            }

            //prepare localized models
            if (!excludeProperties)
                model.Locales = _localizedModelFactory.PrepareLocalizedModels(localizedModelConfiguration);

            return model;
        }

        /// <summary>
        /// Prepare paged customer attribute value list model
        /// </summary>
        /// <param name="searchModel">Customer attribute value search model</param>
        /// <param name="customerAttribute">Customer attribute</param>
        /// <returns>Customer attribute value list model</returns>
        public virtual CustomerAttributeValueListModel PrepareCustomerAttributeValueListModel(CustomerAttributeValueSearchModel searchModel,
            CustomerAttribute customerAttribute)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (customerAttribute == null)
                throw new ArgumentNullException(nameof(customerAttribute));

            //get customer attribute values
            var customerAttributeValues = _customerAttributeService
                .GetCustomerAttributeValues(customerAttribute.Id).ToPagedList(searchModel);

            //prepare list model
            var model = new CustomerAttributeValueListModel().PrepareToGrid(searchModel, customerAttributeValues, () =>
            {
                //fill in model values from the entity
                return customerAttributeValues.Select(value => value.ToModel<CustomerAttributeValueModel>());
            });

            return model;
        }

        /// <summary>
        /// Prepare customer attribute value model
        /// </summary>
        /// <param name="model">Customer attribute value model</param>
        /// <param name="customerAttribute">Customer attribute</param>
        /// <param name="customerAttributeValue">Customer attribute value</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>Customer attribute value model</returns>
        public virtual CustomerAttributeValueModel PrepareCustomerAttributeValueModel(CustomerAttributeValueModel model,
            CustomerAttribute customerAttribute, CustomerAttributeValue customerAttributeValue, bool excludeProperties = false)
        {
            if (customerAttribute == null)
                throw new ArgumentNullException(nameof(customerAttribute));

            Action<CustomerAttributeValueLocalizedModel, int> localizedModelConfiguration = null;

            if (customerAttributeValue != null)
            {
                //fill in model values from the entity
                model = model ?? customerAttributeValue.ToModel<CustomerAttributeValueModel>();

                //define localized model configuration action
                localizedModelConfiguration = (locale, languageId) =>
                {
                    locale.Name = _localizationService.GetLocalized(customerAttributeValue, entity => entity.Name, languageId, false, false);
                };
            }

            model.CustomerAttributeId = customerAttribute.Id;

            //prepare localized models
            if (!excludeProperties)
                model.Locales = _localizedModelFactory.PrepareLocalizedModels(localizedModelConfiguration);

            return model;
        }

        #endregion
    }
}