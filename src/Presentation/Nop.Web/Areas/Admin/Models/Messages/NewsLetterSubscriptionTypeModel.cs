using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Messages;

/// <summary>
/// Represents a newsletter subscription type model
/// </summary>
public partial record NewsLetterSubscriptionTypeModel : BaseNopEntityModel, ILocalizedModel<NewsLetterSubscriptionTypeLocalizedModel>, IStoreMappingSupportedModel
{
    #region Ctor

    public NewsLetterSubscriptionTypeModel()
    {
        Locales = new List<NewsLetterSubscriptionTypeLocalizedModel>();
        SelectedStoreIds = new List<int>();
        AvailableStores = new List<SelectListItem>();
    }

    #endregion

    #region Properties

    [NopResourceDisplayName("Admin.Promotions.NewsLetterSubscriptionType.Fields.Name")]
    public string Name { get; set; }

    [NopResourceDisplayName("Admin.Promotions.NewsLetterSubscriptionType.Fields.TickedByDefault")]
    public bool TickedByDefault { get; set; }

    [NopResourceDisplayName("Admin.Promotions.NewsLetterSubscriptionType.Fields.DisplayOrder")]
    public int DisplayOrder { get; set; }

    public IList<NewsLetterSubscriptionTypeLocalizedModel> Locales { get; set; }

    //store mapping
    [NopResourceDisplayName("Admin.Promotions.NewsLetterSubscriptionType.Fields.LimitedToStores")]
    public IList<int> SelectedStoreIds { get; set; }

    public IList<SelectListItem> AvailableStores { get; set; }

    #endregion
}
