using Nop.Core.Domain.Common;
using Nop.Web.Areas.Admin.Models.Common;

namespace Nop.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the contact form attribute model factory
/// </summary>
public partial interface IContactFormAttributeModelFactory
{
    /// <summary>
    /// Prepare contact form attribute search model
    /// </summary>
    /// <param name="searchModel">Contact form attribute search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the contact form attribute search model
    /// </returns>
    Task<ContactFormAttributeSearchModel> PrepareContactFormAttributeSearchModelAsync(ContactFormAttributeSearchModel searchModel);

    /// <summary>
    /// Prepare paged contact form attribute list model
    /// </summary>
    /// <param name="searchModel">Contact form attribute search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the contact form attribute list model
    /// </returns>
    Task<ContactFormAttributeListModel> PrepareContactFormAttributeListModelAsync(ContactFormAttributeSearchModel searchModel);

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
    Task<ContactFormAttributeModel> PrepareContactFormAttributeModelAsync(ContactFormAttributeModel model,
        ContactFormAttribute contactFormAttribute, bool excludeProperties = false);

    /// <summary>
    /// Prepare paged contact form attribute value list model
    /// </summary>
    /// <param name="searchModel">Contact form attribute value search model</param>
    /// <param name="contactFormAttribute">Contact form attribute</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the contact form attribute value list model
    /// </returns>
    Task<ContactFormAttributeValueListModel> PrepareContactFormAttributeValueListModelAsync(ContactFormAttributeValueSearchModel searchModel,
        ContactFormAttribute contactFormAttribute);

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
    Task<ContactFormAttributeValueModel> PrepareContactFormAttributeValueModelAsync(ContactFormAttributeValueModel model,
        ContactFormAttribute contactFormAttribute, ContactFormAttributeValue contactFormAttributeValue, bool excludeProperties = false);
}