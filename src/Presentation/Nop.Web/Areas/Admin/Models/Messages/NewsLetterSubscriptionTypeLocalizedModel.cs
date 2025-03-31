using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Messages;

/// <summary>
/// Represents a newsletter subscription type localized model
/// </summary>
public partial record NewsLetterSubscriptionTypeLocalizedModel : ILocalizedLocaleModel
{
    public int LanguageId { get; set; }

    [NopResourceDisplayName("Admin.Promotions.NewsLetterSubscriptionType.Fields.Name")]
    public string Name { get; set; }
}
