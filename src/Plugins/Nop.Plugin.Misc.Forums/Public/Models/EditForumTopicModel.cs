using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Plugin.Misc.Forums.Domain;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.Forums.Public.Models;

public record EditForumTopicModel : BaseNopEntityModel
{
    #region Properties

    public bool IsEdit { get; set; }
    public int ForumId { get; set; }
    public string ForumName { get; set; }
    public string ForumSeName { get; set; }
    public int TopicTypeId { get; set; }
    public EditorType ForumEditor { get; set; }
    public string Subject { get; set; }
    public string Text { get; set; }
    public bool IsCustomerAllowedToSetTopicPriority { get; set; }
    public bool IsCustomerAllowedToSubscribe { get; set; }
    public bool Subscribed { get; set; }
    public bool DisplayCaptcha { get; set; }

    public List<SelectListItem> TopicPriorities { get; set; } = new();

    #endregion
}