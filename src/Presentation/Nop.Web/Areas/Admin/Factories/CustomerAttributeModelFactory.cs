using System;
using System.Linq;
using System.Threading.Tasks;
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
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the customer attribute search model
        /// </returns>
        public virtual Task<CustomerAttributeSearchModel> PrepareCustomerAttributeSearchModelAsync(CustomerAttributeSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return Task.FromResult(searchModel);
        }

        /// <summary>
        /// Prepare paged customer attribute list model
        /// </summary>
        /// <param name="searchModel">Customer attribute search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the customer attribute list model
        /// </returns>
        public virtual async Task<CustomerAttributeListModel> PrepareCustomerAttributeListModelAsync(CustomerAttributeSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get customer attributes
            var customerAttributes = (await _customerAttributeService.GetAllCustomerAttributesAsync()).ToPagedList(searchModel);

            //prepare list model
            var model = await new CustomerAttributeListModel().PrepareToGridAsync(searchModel, customerAttributes, () =>
            {
                return customerAttributes.SelectAwait(async attribute =>
                {
                    //fill in model values from the entity
                    var attributeModel = attribute.ToModel<CustomerAttributeModel>();

                    //fill in additional values (not existing in the entity)
                    attributeModel.AttributeControlTypeName = await _localizationService.GetLocalizedEnumAsync(attribute.AttributeControlType);

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
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the customer attribute model
        /// </returns>
        public virtual async Task<CustomerAttributeModel> PrepareCustomerAttributeModelAsync(CustomerAttributeModel model,
            CustomerAttribute customerAttribute, bool excludeProperties = false)
        {
            Func<CustomerAttributeLocalizedModel, int, Task> localizedModelConfiguration = null;

            if (customerAttribute != null)
            {
                //fill in model values from the entity
                model ??= customerAttribute.ToModel<CustomerAttributeModel>();

                //prepare nested search model
                PrepareCustomerAttributeValueSearchModel(model.CustomerAttributeValueSearchModel, customerAttribute);

                //define localized model configuration action
                localizedModelConfiguration = async (locale, languageId) =>
                {
                    locale.Name = await _localizationService.GetLocalizedAsync(customerAttribute, entity => entity.Name, languageId, false, false);
                };
            }

            //prepare localized models
            if (!excludeProperties)
                model.Locales = await _localizedModelFactory.PrepareLocalizedModelsAsync(localizedModelConfiguration);

            return model;
        }

        /// <summary>
        /// Prepare paged customer attribute value list model
        /// </summary>
        /// <param name="searchModel">Customer attribute value search model</param>
        /// <param name="customerAttribute">Customer attribute</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the customer attribute value list model
        /// </returns>
        public virtual async Task<CustomerAttributeValueListModel> PrepareCustomerAttributeValueListModelAsync(CustomerAttributeValueSearchModel searchModel,
            CustomerAttribute customerAttribute)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (customerAttribute == null)
                throw new ArgumentNullException(nameof(customerAttribute));

            //get customer attribute values
            var customerAttributeValues = (await _customerAttributeService
                .GetCustomerAttributeValuesAsync(customerAttribute.Id)).ToPagedList(searchModel);

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
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the customer attribute value model
        /// </returns>
        public virtual async Task<CustomerAttributeValueModel> PrepareCustomerAttributeValueModelAsync(CustomerAttributeValueModel model,
            CustomerAttribute customerAttribute, CustomerAttributeValue customerAttributeValue, bool excludeProperties = false)
        {
            if (customerAttribute == null)
                throw new ArgumentNullException(nameof(customerAttribute));

            Func<CustomerAttributeValueLocalizedModel, int, Task> localizedModelConfiguration = null;

            if (customerAttributeValue != null)
            {
                //fill in model values from the entity
                model ??= customerAttributeValue.ToModel<CustomerAttributeValueModel>();

                //define localized model configuration action
                localizedModelConfiguration = async (locale, languageId) =>
                {
                    locale.Name = await _localizationService.GetLocalizedAsync(customerAttributeValue, entity => entity.Name, languageId, false, false);
                };
            }

            model.CustomerAttributeId = customerAttribute.Id;

            //prepare localized models
            if (!excludeProperties)
                model.Locales = await _localizedModelFactory.PrepareLocalizedModelsAsync(localizedModelConfiguration);

            return model;
        }

        #endregion
    }
}