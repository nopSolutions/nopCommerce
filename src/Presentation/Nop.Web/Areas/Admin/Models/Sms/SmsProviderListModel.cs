using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Sms;

/// <summary>
/// Represents a sms provider list model
/// </summary>
public partial record SmsProviderListModel : BasePagedListModel<SmsProviderModel>;