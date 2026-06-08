using Nop.Core.Domain.Common;
using Nop.Services.Attributes;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Common;
using Nop.Web.Framework.Factories;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the contact form attribute model factory implementation
/// </summary>
public partial class ContactFormAttributeModelFactory : IContactFormAttributeModelFactory
{
    #region Fields

    protected readonly IAttributeService<ContactFormAttribute, ContactFormAttributeValue> _contactFormAttributeService;
    protected readonly ILocalizationService _localizationService;
    protected readonly ILocalizedModelFactory _localizedModelFactory;

    #endregion

    #region Ctor

    public ContactFormAttributeModelFactory(IAttributeService<ContactFormAttribute, ContactFormAttributeValue> contactFormAttributeService,
        ILocalizationService localizationService,
        ILocalizedModelFactory localizedModelFactory)
    {
        _contactFormAttributeService = contactFormAttributeService;
        _localizationService = localizationService;
        _localizedModelFactory = localizedModelFactory;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Prepare contact form attribute value search model
    /// </summary>
    /// <param name="searchModel">Contact form attribute value search model</param>
    /// <param name="contactFormAttribute">Contact form attribute</param>
    /// <returns>Contact form attribute value search model</returns>
    protected virtual ContactFormAttributeValueSearchModel PrepareContactFormAttributeValueSearchModel(ContactFormAttributeValueSearchModel searchModel,
        ContactFormAttribute contactFormAttribute)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        ArgumentNullException.ThrowIfNull(contactFormAttribute);

        searchModel.ContactFormAttributeId = contactFormAttribute.Id;

        //prepare page parameters
        searchModel.SetGridPageSize();

        return searchModel;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Prepare contact form attribute search model
    /// </summary>
    /// <param name="searchModel">Contact form attribute search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the contact form attribute search model
    /// </returns>
    public virtual Task<ContactFormAttributeSearchModel> PrepareContactFormAttributeSearchModelAsync(ContactFormAttributeSearchModel searchModel)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare page parameters
        searchModel.SetGridPageSize();

        return Task.FromResult(searchModel);
    }

    /// <summary>
    /// Prepare paged contact form attribute list model
    /// </summary>
    /// <param name="searchModel">Contact form attribute search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the contact form attribute list model
    /// </returns>
    public virtual async Task<ContactFormAttributeListModel> PrepareContactFormAttributeListModelAsync(ContactFormAttributeSearchModel searchModel)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        //get contact form attributes
        var contactFormAttributes = (await _contactFormAttributeService.GetAllAttributesAsync()).ToPagedList(searchModel);

        //prepare list model
        var model = await new ContactFormAttributeListModel().PrepareToGridAsync(searchModel, contactFormAttributes, () =>
        {
            return contactFormAttributes.SelectAwait(async attribute =>
            {
                //fill in model values from the entity
                var attributeModel = attribute.ToModel<ContactFormAttributeModel>();

                //fill in additional values (not existing in the entity)
                attributeModel.AttributeControlTypeName = await _localizationService.GetLocalizedEnumAsync(attribute.AttributeControlType);

                return attributeModel;
            });
        });

        return model;
    }

    /// <summary>
    /// Prepare contact form attribute model
    /// </summary>
    /// <param name="model">Contact form attribute model</param>
    /// <param name="contactFormAttribute">Contact form attribute</param>
    /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the contact form attribute model
    /// </returns>
    public virtual async Task<ContactFormAttributeModel> PrepareContactFormAttributeModelAsync(ContactFormAttributeModel model,
        ContactFormAttribute contactFormAttribute, bool excludeProperties = false)
    {
        Func<ContactFormAttributeLocalizedModel, int, Task> localizedModelConfiguration = null;

        if (contactFormAttribute != null)
        {
            //fill in model values from the entity
            model ??= contactFormAttribute.ToModel<ContactFormAttributeModel>();

            //prepare nested search model
            PrepareContactFormAttributeValueSearchModel(model.ContactFormAttributeValueSearchModel, contactFormAttribute);

            //define localized model configuration action
            localizedModelConfiguration = async (locale, languageId) =>
            {
                locale.Name = await _localizationService.GetLocalizedAsync(contactFormAttribute, entity => entity.Name, languageId, false, false);
            };
        }

        //prepare localized models
        if (!excludeProperties)
            model.Locales = await _localizedModelFactory.PrepareLocalizedModelsAsync(localizedModelConfiguration);

        return model;
    }

    /// <summary>
    /// Prepare paged contact form attribute value list model
    /// </summary>
    /// <param name="searchModel">Contact form attribute value search model</param>
    /// <param name="contactFormAttribute">Contact form attribute</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the contact form attribute value list model
    /// </returns>
    public virtual async Task<ContactFormAttributeValueListModel> PrepareContactFormAttributeValueListModelAsync(ContactFormAttributeValueSearchModel searchModel,
        ContactFormAttribute contactFormAttribute)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        ArgumentNullException.ThrowIfNull(contactFormAttribute);

        //get contact form attribute values
        var contactFormAttributeValues = (await _contactFormAttributeService
            .GetAttributeValuesAsync(contactFormAttribute.Id)).ToPagedList(searchModel);

        //prepare list model
        var model = new ContactFormAttributeValueListModel().PrepareToGrid(searchModel, contactFormAttributeValues, () =>
        {
            //fill in model values from the entity
            return contactFormAttributeValues.Select(value => value.ToModel<ContactFormAttributeValueModel>());
        });

        return model;
    }

    /// <summary>
    /// Prepare contact form attribute value model
    /// </summary>
    /// <param name="model">Contact form attribute value model</param>
    /// <param name="contactFormAttribute">Contact form attribute</param>
    /// <param name="contactFormAttributeValue">Contact form attribute value</param>
    /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the contact form attribute value model
    /// </returns>
    public virtual async Task<ContactFormAttributeValueModel> PrepareContactFormAttributeValueModelAsync(ContactFormAttributeValueModel model,
        ContactFormAttribute contactFormAttribute, ContactFormAttributeValue contactFormAttributeValue, bool excludeProperties = false)
    {
        ArgumentNullException.ThrowIfNull(contactFormAttribute);

        Func<ContactFormAttributeValueLocalizedModel, int, Task> localizedModelConfiguration = null;

        if (contactFormAttributeValue != null)
        {
            //fill in model values from the entity
            model ??= contactFormAttributeValue.ToModel<ContactFormAttributeValueModel>();

            //define localized model configuration action
            localizedModelConfiguration = async (locale, languageId) =>
            {
                locale.Name = await _localizationService.GetLocalizedAsync(contactFormAttributeValue, entity => entity.Name, languageId, false, false);
            };
        }

        model.AttributeId = contactFormAttribute.Id;

        //prepare localized models
        if (!excludeProperties)
            model.Locales = await _localizedModelFactory.PrepareLocalizedModelsAsync(localizedModelConfiguration);

        return model;
    }

    #endregion
}