using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Messages;

/// <summary>
/// Represents a requeue queued emails model
/// </summary>
public partial record RequeueEmailsModel : BaseNopModel
{
    [NopResourceDisplayName("Admin.System.QueuedEmails.Fields.SendImmediately")]
    public bool SendImmediately { get; set; }

    [NopResourceDisplayName("Admin.System.QueuedEmails.Fields.DontSendBeforeDate")]
    [UIHint("DateTimeNullable")]
    public DateTime? DontSendBeforeDate { get; set; }

    public string SelectedQueuedEmailIds { get; set; }
}
