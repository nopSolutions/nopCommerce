using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Messages;

/// <summary>
/// Represents a newsletter subscription model
/// </summary>
public partial record NewsLetterSubscriptionModel : BaseNopEntityModel
{
    #region Ctor

    public NewsLetterSubscriptionModel()
    {
        AvailableNewsLetterSubscriptionTypes = new List<SelectListItem>();
        AvailableNewsLetterSubscriptionStores = new List<SelectListItem>();
        AvailableNewsLetterSubscriptionLanguages = new List<SelectListItem>();
    }

    #endregion

    #region Properties

    [DataType(DataType.EmailAddress)]
    [NopResourceDisplayName("Admin.Promotions.NewsLetterSubscription.Fields.Email")]
    public string Email { get; set; }

    public string SubscriptionTypeName { get; set; }

    [NopResourceDisplayName("Admin.Promotions.NewsLetterSubscription.Fields.SubscriptionType")]
    public int SelectedNewsLetterSubscriptionTypeId { get; set; }
    public IList<SelectListItem> AvailableNewsLetterSubscriptionTypes { get; set; }

    [NopResourceDisplayName("Admin.Promotions.NewsLetterSubscription.Fields.Active")]
    public bool Active { get; set; }

    public string StoreName { get; set; }

    public IList<SelectListItem> AvailableNewsLetterSubscriptionStores { get; set; }

    [NopResourceDisplayName("Admin.Promotions.NewsLetterSubscription.Fields.Store")]
    public int SelectedNewsLetterSubscriptionStoreId { get; set; }

    public string LanguageName { get; set; }
    public IList<SelectListItem> AvailableNewsLetterSubscriptionLanguages { get; set; }

    [NopResourceDisplayName("Admin.Promotions.NewsLetterSubscription.Fields.Language")]
    public int SelectedNewsLetterSubscriptionLanguageId { get; set; }

    [NopResourceDisplayName("Admin.Promotions.NewsLetterSubscription.Fields.CreatedOn")]
    public string CreatedOn { get; set; }

    #endregion
}