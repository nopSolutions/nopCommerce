using System;
using System.Linq;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Services.Common;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Extensions;
using Nop.Web.Areas.Admin.Models.Common;
using Nop.Web.Framework.Extensions;
using Nop.Web.Framework.Factories;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the address attribute model factory implementation
    /// </summary>
    public partial class AddressAttributeModelFactory : IAddressAttributeModelFactory
    {
        #region Fields

        private readonly IAddressAttributeService _addressAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedModelFactory _localizedModelFactory;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public AddressAttributeModelFactory(IAddressAttributeService addressAttributeService,
            ILocalizationService localizationService,
            ILocalizedModelFactory localizedModelFactory,
            IWorkContext workContext)
        {
            this._addressAttributeService = addressAttributeService;
            this._localizationService = localizationService;
            this._localizedModelFactory = localizedModelFactory;
            this._workContext = workContext;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare address attribute search model
        /// </summary>
        /// <param name="model">Address attribute search model</param>
        /// <returns>Address attribute search model</returns>
        public virtual AddressAttributeSearchModel PrepareAddressAttributeSearchModel(AddressAttributeSearchModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            //prepare page parameters
            model.SetGridPageSize();

            return model;
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
            var addressAttributes = _addressAttributeService.GetAllAddressAttributes();

            //prepare grid model
            var model = new AddressAttributeListModel
            {
                Data = addressAttributes.PaginationByRequestModel(searchModel).Select(attribute =>
                {
                    //fill in model values from the entity
                    var attributeModel = attribute.ToModel();

                    //fill in additional values (not existing in the entity)
                    attributeModel.AttributeControlTypeName = attribute.AttributeControlType.GetLocalizedEnum(_localizationService, _workContext);

                    return attributeModel;
                }),
                Total = addressAttributes.Count
            };

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
                model = model ?? addressAttribute.ToModel();

                //prepare nested search model
                PrepareAddressAttributeValueSearchModel(model.AddressAttributeValueSearchModel, addressAttribute);

                //define localized model configuration action
                localizedModelConfiguration = (locale, languageId) =>
                {
                    locale.Name = addressAttribute.GetLocalized(entity => entity.Name, languageId, false, false);
                };
            }

            //prepare localized models
            if (!excludeProperties)
                model.Locales = _localizedModelFactory.PrepareLocalizedModels(localizedModelConfiguration);

            return model;
        }

        /// <summary>
        /// Prepare address attribute value search model
        /// </summary>
        /// <param name="model">Address attribute value search model</param>
        /// <param name="addressAttribute">Address attribute</param>
        /// <returns>Address attribute value search model</returns>
        public virtual AddressAttributeValueSearchModel PrepareAddressAttributeValueSearchModel(AddressAttributeValueSearchModel model,
            AddressAttribute addressAttribute)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (addressAttribute == null)
                throw new ArgumentNullException(nameof(addressAttribute));

            model.AddressAttributeId = addressAttribute.Id;

            //prepare page parameters
            model.SetGridPageSize();

            return model;
        }

        /// <summary>
        /// Prepare paged address attribute value list model
        /// </summary>
        /// <param name="searchModel">Address attribute value search model</param>
        /// <param name="AddressAttribute">Address attribute</param>
        /// <returns>Address attribute value list model</returns>
        public virtual AddressAttributeValueListModel PrepareAddressAttributeValueListModel(AddressAttributeValueSearchModel searchModel,
            AddressAttribute addressAttribute)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (addressAttribute == null)
                throw new ArgumentNullException(nameof(addressAttribute));

            //get address attribute values
            var addressAttributeValues = _addressAttributeService.GetAddressAttributeValues(addressAttribute.Id);

            //prepare grid model
            var model = new AddressAttributeValueListModel
            {
                //fill in model values from the entity
                Data = addressAttributeValues.PaginationByRequestModel(searchModel).Select(value => new AddressAttributeValueModel
                {
                    Id = value.Id,
                    AddressAttributeId = value.AddressAttributeId,
                    Name = value.Name,
                    IsPreSelected = value.IsPreSelected,
                    DisplayOrder = value.DisplayOrder,
                }),
                Total = addressAttributeValues.Count
            };

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
                model = model ?? new AddressAttributeValueModel
                {
                    Name = addressAttributeValue.Name,
                    IsPreSelected = addressAttributeValue.IsPreSelected,
                    DisplayOrder = addressAttributeValue.DisplayOrder
                };

                //define localized model configuration action
                localizedModelConfiguration = (locale, languageId) =>
                {
                    locale.Name = addressAttributeValue.GetLocalized(entity => entity.Name, languageId, false, false);
                };
            }

            model.AddressAttributeId = addressAttribute.Id;

            //prepare localized models
            if (!excludeProperties)
                model.Locales = _localizedModelFactory.PrepareLocalizedModels(localizedModelConfiguration);

            return model;
        }

        #endregion
    }
}