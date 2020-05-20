using System;
using System.Collections.Generic;
using System.Linq;
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
        /// <returns>Address attribute search model</returns>
        public virtual AddressAttributeSearchModel PrepareAddressAttributeSearchModel(AddressAttributeSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged address attribute list model
        /// </summary>
        /// <param name="searchModel">Address attribute search model</param>
        /// <returns>Address attribute list model</returns>
        public virtual AddressAttributeListModel PrepareAddressAttributeListModel(AddressAttributeSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get address attributes
            var addressAttributes = _addressAttributeService.GetAllAddressAttributes().ToPagedList(searchModel);

            //prepare grid model
            var model = new AddressAttributeListModel().PrepareToGrid(searchModel, addressAttributes, () =>
            {
                return addressAttributes.Select(attribute =>
                {
                    //fill in model values from the entity
                    var attributeModel = attribute.ToModel<AddressAttributeModel>();

                    //fill in additional values (not existing in the entity)
                    attributeModel.AttributeControlTypeName =
                        _localizationService.GetLocalizedEnum(attribute.AttributeControlType);

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
        /// <returns>Address attribute model</returns>
        public virtual AddressAttributeModel PrepareAddressAttributeModel(AddressAttributeModel model,
            AddressAttribute addressAttribute, bool excludeProperties = false)
        {
            Action<AddressAttributeLocalizedModel, int> localizedModelConfiguration = null;

            if (addressAttribute != null)
            {
                //fill in model values from the entity
                model ??= addressAttribute.ToModel<AddressAttributeModel>();

                //prepare nested search model
                PrepareAddressAttributeValueSearchModel(model.AddressAttributeValueSearchModel, addressAttribute);

                //define localized model configuration action
                localizedModelConfiguration = (locale, languageId) =>
                {
                    locale.Name = _localizationService.GetLocalized(addressAttribute, entity => entity.Name, languageId, false, false);
                };
            }

            //prepare localized models
            if (!excludeProperties)
                model.Locales = _localizedModelFactory.PrepareLocalizedModels(localizedModelConfiguration);

            return model;
        }

        /// <summary>
        /// Prepare paged address attribute value list model
        /// </summary>
        /// <param name="searchModel">Address attribute value search model</param>
        /// <param name="addressAttribute">Address attribute</param>
        /// <returns>Address attribute value list model</returns>
        public virtual AddressAttributeValueListModel PrepareAddressAttributeValueListModel(AddressAttributeValueSearchModel searchModel,
            AddressAttribute addressAttribute)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (addressAttribute == null)
                throw new ArgumentNullException(nameof(addressAttribute));

            //get address attribute values
            var addressAttributeValues = _addressAttributeService.GetAddressAttributeValues(addressAttribute.Id).ToPagedList(searchModel);

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
        /// <returns>Address attribute value model</returns>
        public virtual AddressAttributeValueModel PrepareAddressAttributeValueModel(AddressAttributeValueModel model,
            AddressAttribute addressAttribute, AddressAttributeValue addressAttributeValue, bool excludeProperties = false)
        {
            if (addressAttribute == null)
                throw new ArgumentNullException(nameof(addressAttribute));

            Action<AddressAttributeValueLocalizedModel, int> localizedModelConfiguration = null;

            if (addressAttributeValue != null)
            {
                //fill in model values from the entity
                model ??= addressAttributeValue.ToModel<AddressAttributeValueModel>();

                //define localized model configuration action
                localizedModelConfiguration = (locale, languageId) =>
                {
                    locale.Name = _localizationService.GetLocalized(addressAttributeValue, entity => entity.Name, languageId, false, false);
                };
            }

            model.AddressAttributeId = addressAttribute.Id;

            //prepare localized models
            if (!excludeProperties)
                model.Locales = _localizedModelFactory.PrepareLocalizedModels(localizedModelConfiguration);

            return model;
        }

        /// <summary>
        /// Prepare custom address attributes
        /// </summary>
        /// <param name="models">List of address attribute models</param>
        /// <param name="address">Address</param>
        public virtual void PrepareCustomAddressAttributes(IList<AddressModel.AddressAttributeModel> models, Address address)
        {
            if (models == null)
                throw new ArgumentNullException(nameof(models));

            var attributes = _addressAttributeService.GetAllAddressAttributes();
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
                    var attributeValues = _addressAttributeService.GetAddressAttributeValues(attribute.Id);
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
                            var selectedValues = _addressAttributeParser.ParseAddressAttributeValues(selectedAddressAttributes);
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