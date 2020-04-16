using System;
using System.Linq;
using Nop.Core.Domain.Customers;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Customers;
using Nop.Web.Framework.Factories;
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

            return searchModel;
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
                model ??= customerAttribute.ToModel<CustomerAttributeModel>();

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
                model ??= customerAttributeValue.ToModel<CustomerAttributeValueModel>();

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