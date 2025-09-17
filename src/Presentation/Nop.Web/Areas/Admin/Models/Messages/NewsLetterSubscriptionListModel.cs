using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Messages;

/// <summary>
/// Represents a newsletter subscription list model
/// </summary>
public partial record NewsLetterSubscriptionListModel : BasePagedListModel<NewsLetterSubscriptionModel>;