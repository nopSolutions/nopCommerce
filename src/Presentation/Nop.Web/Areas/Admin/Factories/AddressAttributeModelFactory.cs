using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Services.Common;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Common;
using Nop.Web.Framework.Factories;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the address attribute model factory implementation
    /// </summary>
    public partial class AddressAttributeModelFactory : IAddressAttributeModelFactory
    {
        #region Fields

        private readonly IAddressAttributeParser _addressAttributeParser;
        private readonly IAddressAttributeService _addressAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedModelFactory _localizedModelFactory;

        #endregion

        #region Ctor

        public AddressAttributeModelFactory(IAddressAttributeParser addressAttributeParser,
            IAddressAttributeService addressAttributeService,
            ILocalizationService localizationService,
            ILocalizedModelFactory localizedModelFactory)
        {
            _addressAttributeParser = addressAttributeParser;
            _addressAttributeService = addressAttributeService;
            _localizationService = localizationService;
            _localizedModelFactory = localizedModelFactory;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Prepare address attribute value search model
        /// </summary>
        /// <param name="searchModel">Address attribute value search model</param>
        /// <param name="addressAttribute">Address attribute</param>
        /// <returns>Address attribute value search model</returns>
        protected virtual AddressAttributeValueSearchModel PrepareAddressAttributeValueSearchModel(AddressAttributeValueSearchModel searchModel, AddressAttribute addressAttribute)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (addressAttribute == null)
                throw new ArgumentNullException(nameof(addressAttribute));

            searchModel.AddressAttributeId = addressAttribute.Id;

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare address attribute search model
        /// </summary>
        /// <param name="searchModel">Address attribute search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the address attribute search model
        /// </returns>
        public virtual Task<AddressAttributeSearchModel> PrepareAddressAttributeSearchModelAsync(AddressAttributeSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return Task.FromResult(searchModel);
        }

        /// <summary>
        /// Prepare paged address attribute list model
        /// </summary>
        /// <param name="searchModel">Address attribute search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the address attribute list model
        /// </returns>
        public virtual async Task<AddressAttributeListModel> PrepareAddressAttributeListModelAsync(AddressAttributeSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get address attributes
            var addressAttributes = (await _addressAttributeService.GetAllAddressAttributesAsync()).ToPagedList(searchModel);

            //prepare grid model
            var model = await new AddressAttributeListModel().PrepareToGridAsync(searchModel, addressAttributes, () =>
            {
                return addressAttributes.SelectAwait(async attribute =>
                {
                    //fill in model values from the entity
                    var attributeModel = attribute.ToModel<AddressAttributeModel>();

                    //fill in additional values (not existing in the entity)
                    attributeModel.AttributeControlTypeName = await _localizationService.GetLocalizedEnumAsync(attribute.AttributeControlType);

                    return attributeModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare address attribute model
        /// </summary>
        /// <param name="model">Address attribute model</param>
        /// <param name="addressAttribute">Address attribute</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the address attribute model
        /// </returns>
        public virtual async Task<AddressAttributeModel> PrepareAddressAttributeModelAsync(AddressAttributeModel model,
            AddressAttribute addressAttribute, bool excludeProperties = false)
        {
            Func<AddressAttributeLocalizedModel, int, Task> localizedModelConfiguration = null;

            if (addressAttribute != null)
            {
                //fill in model values from the entity
                model ??= addressAttribute.ToModel<AddressAttributeModel>();

                //prepare nested search model
                PrepareAddressAttributeValueSearchModel(model.AddressAttributeValueSearchModel, addressAttribute);

                //define localized model configuration action
                localizedModelConfiguration = async (locale, languageId) =>
                {
                    locale.Name = await _localizationService.GetLocalizedAsync(addressAttribute, entity => entity.Name, languageId, false, false);
                };
            }

            //prepare localized models
            if (!excludeProperties)
                model.Locales = await _localizedModelFactory.PrepareLocalizedModelsAsync(localizedModelConfiguration);

            return model;
        }

        /// <summary>
        /// Prepare paged address attribute value list model
        /// </summary>
        /// <param name="searchModel">Address attribute value search model</param>
        /// <param name="addressAttribute">Address attribute</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the address attribute value list model
        /// </returns>
        public virtual async Task<AddressAttributeValueListModel> PrepareAddressAttributeValueListModelAsync(AddressAttributeValueSearchModel searchModel,
            AddressAttribute addressAttribute)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (addressAttribute == null)
                throw new ArgumentNullException(nameof(addressAttribute));

            //get address attribute values
            var addressAttributeValues = (await _addressAttributeService.GetAddressAttributeValuesAsync(addressAttribute.Id)).ToPagedList(searchModel);

            //prepare grid model
            var model = new AddressAttributeValueListModel().PrepareToGrid(searchModel, addressAttributeValues, () =>
            {
                //fill in model values from the entity
                return addressAttributeValues.Select(value => value.ToModel<AddressAttributeValueModel>());
            });

            return model;
        }

        /// <summary>
        /// Prepare address attribute value model
        /// </summary>
        /// <param name="model">Address attribute value model</param>
        /// <param name="addressAttribute">Address attribute</param>
        /// <param name="addressAttributeValue">Address attribute value</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the address attribute value model
        /// </returns>
        public virtual async Task<AddressAttributeValueModel> PrepareAddressAttributeValueModelAsync(AddressAttributeValueModel model,
            AddressAttribute addressAttribute, AddressAttributeValue addressAttributeValue, bool excludeProperties = false)
        {
            if (addressAttribute == null)
                throw new ArgumentNullException(nameof(addressAttribute));

            Func<AddressAttributeValueLocalizedModel, int, Task> localizedModelConfiguration = null;

            if (addressAttributeValue != null)
            {
                //fill in model values from the entity
                model ??= addressAttributeValue.ToModel<AddressAttributeValueModel>();

                //define localized model configuration action
                localizedModelConfiguration = async (locale, languageId) =>
                {
                    locale.Name = await _localizationService.GetLocalizedAsync(addressAttributeValue, entity => entity.Name, languageId, false, false);
                };
            }

            model.AddressAttributeId = addressAttribute.Id;

            //prepare localized models
            if (!excludeProperties)
                model.Locales = await _localizedModelFactory.PrepareLocalizedModelsAsync(localizedModelConfiguration);

            return model;
        }

        /// <summary>
        /// Prepare custom address attributes
        /// </summary>
        /// <param name="models">List of address attribute models</param>
        /// <param name="address">Address</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task PrepareCustomAddressAttributesAsync(IList<AddressModel.AddressAttributeModel> models, Address address)
        {
            if (models == null)
                throw new ArgumentNullException(nameof(models));

            var attributes = await _addressAttributeService.GetAllAddressAttributesAsync();
            foreach (var attribute in attributes)
            {
                var attributeModel = new AddressModel.AddressAttributeModel
                {
                    Id = attribute.Id,
                    Name = attribute.Name,
                    IsRequired = attribute.IsRequired,
                    AttributeControlType = attribute.AttributeControlType,
                };

                if (attribute.ShouldHaveValues())
                {
                    //values
                    var attributeValues = await _addressAttributeService.GetAddressAttributeValuesAsync(attribute.Id);
                    foreach (var attributeValue in attributeValues)
                    {
                        var attributeValueModel = new AddressModel.AddressAttributeValueModel
                        {
                            Id = attributeValue.Id,
                            Name = attributeValue.Name,
                            IsPreSelected = attributeValue.IsPreSelected
                        };
                        attributeModel.Values.Add(attributeValueModel);
                    }
                }

                //set already selected attributes
                var selectedAddressAttributes = address?.CustomAttributes;
                switch (attribute.AttributeControlType)
                {
                    case AttributeControlType.DropdownList:
                    case AttributeControlType.RadioList:
                    case AttributeControlType.Checkboxes:
                    {
                        if (!string.IsNullOrEmpty(selectedAddressAttributes))
                        {
                            //clear default selection
                            foreach (var item in attributeModel.Values)
                                item.IsPreSelected = false;

                            //select new values
                            var selectedValues = await _addressAttributeParser.ParseAddressAttributeValuesAsync(selectedAddressAttributes);
                            foreach (var attributeValue in selectedValues)
                                foreach (var item in attributeModel.Values)
                                    if (attributeValue.Id == item.Id)
                                        item.IsPreSelected = true;
                        }
                    }
                    break;
                    case AttributeControlType.ReadonlyCheckboxes:
                    {
                        //do nothing
                        //values are already pre-set
                    }
                    break;
                    case AttributeControlType.TextBox:
                    case AttributeControlType.MultilineTextbox:
                    {
                        if (!string.IsNullOrEmpty(selectedAddressAttributes))
                        {
                            var enteredText = _addressAttributeParser.ParseValues(selectedAddressAttributes, attribute.Id);
                            if (enteredText.Any())
                                attributeModel.DefaultValue = enteredText[0];
                        }
                    }
                    break;
                    case AttributeControlType.ColorSquares:
                    case AttributeControlType.ImageSquares:
                    case AttributeControlType.Datepicker:
                    case AttributeControlType.FileUpload:
                    default:
                        //not supported attribute control types
                        break;
                }

                models.Add(attributeModel);
            }
        }

        #endregion
    }
}