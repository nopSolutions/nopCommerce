using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Topics;

/// <summary>
/// Represents a topic model
/// </summary>
public partial record TopicModel : BaseNopEntityModel, IAclSupportedModel, ILocalizedModel<TopicLocalizedModel>, IStoreMappingSupportedModel
{
    #region Ctor

    public TopicModel()
    {
        AvailableTopicTemplates = new List<SelectListItem>();
        Locales = new List<TopicLocalizedModel>();

        SelectedCustomerRoleIds = new List<int>();
        AvailableCustomerRoles = new List<SelectListItem>();

        SelectedStoreIds = new List<int>();
        AvailableStores = new List<SelectListItem>();
    }

    #endregion

    #region Properties

    [NopResourceDisplayName("Admin.ContentManagement.Topics.Fields.SystemName")]
    public string SystemName { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Topics.Fields.IncludeInSitemap")]
    public bool IncludeInSitemap { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Topics.Fields.IncludeInTopMenu")]
    public bool IncludeInTopMenu { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Topics.Fields.IncludeInFooterColumn1")]
    public bool IncludeInFooterColumn1 { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Topics.Fields.IncludeInFooterColumn2")]
    public bool IncludeInFooterColumn2 { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Topics.Fields.IncludeInFooterColumn3")]
    public bool IncludeInFooterColumn3 { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Topics.Fields.DisplayOrder")]
    public int DisplayOrder { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Topics.Fields.AccessibleWhenStoreClosed")]
    public bool AccessibleWhenStoreClosed { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Topics.Fields.IsPasswordProtected")]
    public bool IsPasswordProtected { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Topics.Fields.Password")]
    public string Password { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Topics.Fields.URL")]
    public string Url { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Topics.Fields.Title")]
    public string Title { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Topics.Fields.Body")]
    public string Body { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Topics.Fields.Published")]
    public bool Published { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Topics.Fields.TopicTemplate")]
    public int TopicTemplateId { get; set; }

    public IList<SelectListItem> AvailableTopicTemplates { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Topics.Fields.MetaKeywords")]
    public string MetaKeywords { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Topics.Fields.MetaDescription")]
    public string MetaDescription { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Topics.Fields.MetaTitle")]
    public string MetaTitle { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Topics.Fields.SeName")]
    public string SeName { get; set; }

    public IList<TopicLocalizedModel> Locales { get; set; }

    //store mapping
    [NopResourceDisplayName("Admin.ContentManagement.Topics.Fields.LimitedToStores")]
    public IList<int> SelectedStoreIds { get; set; }

    public IList<SelectListItem> AvailableStores { get; set; }

    //ACL (customer roles)
    public IList<int> SelectedCustomerRoleIds { get; set; }

    public IList<SelectListItem> AvailableCustomerRoles { get; set; }

    public string TopicName { get; set; }

    #endregion
}

public partial record TopicLocalizedModel : ILocalizedLocaleModel
{
    public int LanguageId { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Topics.Fields.Title")]
    public string Title { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Topics.Fields.Body")]
    public string Body { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Topics.Fields.MetaKeywords")]
    public string MetaKeywords { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Topics.Fields.MetaDescription")]
    public string MetaDescription { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Topics.Fields.MetaTitle")]
    public string MetaTitle { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Topics.Fields.SeName")]
    public string SeName { get; set; }
}